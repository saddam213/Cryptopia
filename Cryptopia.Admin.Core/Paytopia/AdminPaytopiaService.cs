using Cryptopia.Admin.Common.Paytopia;
using Cryptopia.Common.Cache;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.Paytopia;
using Cryptopia.Common.Trade;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Infrastructure.Common.Results;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Core.Paytopia
{
	public class AdminPaytopiaService : IAdminPaytopiaService
	{
		public ICacheService CacheService { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }
		public IPoolDataContextFactory PoolDataContextFactory { get; set; }
		public ITradeService TradeService { get; set; }

		public async Task<DataTablesResponse> GetPayments(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.PaytopiaPayments
					.AsNoTracking()
					.Select(payment => new PaymentDatatableModel
					{
						Id = payment.Id,
						Type = payment.PaytopiaItem.Type,
						Symbol = payment.PaytopiaItem.CurrencyId.ToString(),
						CurrencyId = payment.PaytopiaItem.CurrencyId,
						Amount = payment.Amount,
						Status = payment.Status,
						UserName = payment.User.UserName,
						IsAnonymous = payment.IsAnonymous,
						Begins = payment.Begins,
						Ends = payment.Ends,
						Timestamp = payment.Timestamp,
						TransferId = payment.TransferId,
						RefundId = payment.RefundId
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<PaytopiaPaymentModel> GetPayment(int id)
		{
			PaytopiaPaymentModel item;
			string currencySymbol;
			dynamic refCurrency;
			Entity.Pool refPool;

			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				item = await context.PaytopiaPayments
					.AsNoTracking()
					.Where(x => x.Id == id)
					.Select(payment => new PaytopiaPaymentModel
					{
						Id = payment.Id,
						Type = payment.PaytopiaItem.Type,
						CurrencyId = payment.PaytopiaItem.CurrencyId,
						Amount = payment.Amount,
						Status = payment.Status,
						UserName = payment.User.UserName,
						IsAnonymous = payment.IsAnonymous,
						Begins = payment.Begins,
						Ends = payment.Ends,
						Timestamp = payment.Timestamp,
						TransferId = payment.TransferId,
						RefundId = payment.RefundId,
						ReferenceCode = payment.ReferenceCode,
						ReferenceId = payment.ReferenceId,
						RefundReason = payment.RefundReason,
						RequestData = payment.RequestData,
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);

			}

			using (var exchangeContext = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				currencySymbol = await exchangeContext.Currency.Where(c => c.Id == item.CurrencyId).Select(c => c.Symbol).FirstOrDefaultNoLockAsync();
				refCurrency = await exchangeContext.Currency.Where(c => c.Id == item.ReferenceId).Select(c => new {
					Name = c.Name,
					AlgoType = c.Info.AlgoType,
					Symbol = c.Symbol
				}).FirstOrDefaultNoLockAsync();
			}

			using (var poolContext = PoolDataContextFactory.CreateContext())
			{

				refPool = await poolContext.Pool.Where(p => p.IsEnabled && p.Id == item.ReferenceId).FirstOrDefaultNoLockAsync();
			}

			item.Symbol = currencySymbol;

			if (item.ReferenceId > 0)
			{
				if (item.Type == PaytopiaItemType.FeaturedCurrency || item.Type == PaytopiaItemType.LottoSlot || item.Type == PaytopiaItemType.RewardSlot || item.Type == PaytopiaItemType.TipSlot)
				{
					if (refCurrency != null)
					{
						item.ReferenceName = refCurrency.Name;
						item.ReferenceAlgo = refCurrency.AlgoType;
						item.ReferenceSymbol = refCurrency.Symbol;
					}
				}
				else if (item.Type == PaytopiaItemType.FeaturedPool || item.Type == PaytopiaItemType.PoolListing)
				{
					if (refPool != null)
					{
						item.ReferenceName = refPool.Name;
						item.ReferenceAlgo = refPool.AlgoType;
						item.ReferenceSymbol = refPool.Symbol;
					}
				}
			}

			return item;
		}

		public async Task<IWriterResult> UpdatePaytopiaPayment(string adminUserId, AdminUpdatePaytopiaPaymentModel model)
		{
			var amount = 0m;
			var currencyId = 0;
			var isRefund = false;
			var userId = string.Empty;
			using (var context = DataContextFactory.CreateContext())
			{
				var projection = await context.PaytopiaPayments
					.Where(x => x.Id == model.PaymentId)
					.Select(x => new {
						Payment = x,
						PaymentItem = x.PaytopiaItem
					})
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);

				var payment = projection.Payment;
				payment.PaytopiaItem = projection.PaymentItem;

				if (payment == null)
					return new WriterResult(false, $"Payment #{model.PaymentId} not found.");

				if (payment.Status != PaytopiaPaymentStatus.Pending)
					return new WriterResult(false, "You can only update 'Pending' payments items.");

				if (payment.Status == PaytopiaPaymentStatus.Pending && model.Status == PaytopiaPaymentStatus.Refunded)
				{
					if (payment.TransferId == 0)
						return new WriterResult(false, "No transaction found to refund.");

					userId = payment.UserId;
					amount = payment.Amount;
					currencyId = payment.PaytopiaItem.CurrencyId;
					payment.Status = PaytopiaPaymentStatus.Refunded;
					payment.RefundReason = model.Reason;
					await context.SaveChangesAsync().ConfigureAwait(false);
					isRefund = true;
				}
				else if (payment.Status == PaytopiaPaymentStatus.Pending && model.Status == PaytopiaPaymentStatus.Complete)
				{
					payment.Status = PaytopiaPaymentStatus.Complete;
                    context.LogActivity(adminUserId, $"Paytopia Payment {payment.Id} updated to complete");
					await context.SaveChangesAsync().ConfigureAwait(false);

					await CacheService.InvalidateAsync(CacheKey.ShareholderPaytopiaFeeInfo());
				}
			}

			if (isRefund)
			{
				var transferResult = await TradeService.CreateTransfer(Constant.SYSTEM_USER.ToString(), new CreateTransferModel
				{
					Amount = amount,
					CurrencyId = currencyId,
					Receiver = userId,
					TransferType = TransferType.Paytopia
				});
				if (transferResult.IsError)
					return new WriterResult(false, transferResult.Error);

				using (var context = DataContextFactory.CreateContext())
				{
					var payment =
						await
							context.PaytopiaPayments.Where(x => x.Id == model.PaymentId).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (payment == null)
						return new WriterResult(false);

					payment.RefundId = transferResult.TransferId;
                    context.LogActivity(adminUserId, $"Paytopia Refund. Payment: {payment.Id}, Refund Id: {payment.RefundId}");
					await context.SaveChangesAsync().ConfigureAwait(false);
				}
				return new WriterResult(true, $"Successfully refunded user for payment #{model.PaymentId}");
			}

			return new WriterResult(true, $"Successfully updated payment status for payment #{model.PaymentId} to {model.Status}");
		}
	}
}
