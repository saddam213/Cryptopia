using Cryptopia.Common.TermDeposits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Enums;

namespace Cryptopia.Core.TermDeposits
{
	public class TermDepositReader : ITermDepositReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<DataTablesResponse> AdminGetDeposits(string userId, DataTablesModel model)
		{
			var currentUser = new Guid(userId);
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.TermDeposit
					.AsNoTracking()
					.OrderByDescending(x => x.Id)
					.Select(x => new
					{
						Id = x.Id,
						Withdraw = x.WithdrawId,
						Name = x.TermDepositItem.Name,
						Investment = x.Withdraw.Amount,
						InterestRate = x.TermDepositItem.InterestRate,
						Started = x.TermBegin,
						End = x.TermEnd,
						Next = x.NextPayout,
						Status = x.Status
					});

				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		public async Task<UpdateTermDepositPaymentModel> AdminGetPayout(string userId, int id)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var payment = await context.TermDepositPayment
					.AsNoTracking()
					.Where(x => x.Id == id)
					.Select(x => new UpdateTermDepositPaymentModel
					{
						Amount = x.Amount,
						Name = x.TermDeposit.TermDepositItem.Name,
						PaymentId = x.Id,
						TransactionId = x.TransactionId,
						Type = x.Type,
						UserName = x.TermDeposit.User.UserName,
						UserId = x.TermDeposit.UserId
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (payment == null)
					return null;

				var userAddress =
					await
						context.Address.FirstOrDefaultNoLockAsync(x => x.UserId == payment.UserId && x.CurrencyId == Constant.DOTCOIN_ID);
				if (userAddress == null)
					return payment;

				payment.Address = userAddress.AddressHash;
				return payment;
			}
		}

		public async Task<DataTablesResponse> AdminGetPayouts(string userId, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.TermDepositPayment
					.AsNoTracking()
					.OrderByDescending(x => x.Id)
					.Select(x => new
					{
						Id = x.Id,
						TermDepositId = x.TermDepositId,
						UserName = x.TermDeposit.User.UserName,
						Investment = x.TermDeposit.Withdraw.Amount,
						InterestRate = x.TermDeposit.TermDepositItem.InterestRate,
						Payment = x.Amount,
						Type = x.Type,
						Timestamp = x.Timestamp,
						TransactionId = x.TransactionId
					});

				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		public async Task<DataTablesResponse> GetClosedDeposits(string userId, DataTablesModel model)
		{
			var currentUser = new Guid(userId);
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.TermDeposit
					.AsNoTracking()
					.Where(
						x =>
							x.UserId == currentUser &&
							(x.Status == Enums.TermDepositStatus.Closed || x.Status == Enums.TermDepositStatus.Canceled))
					.OrderByDescending(x => x.TermBegin)
					.Select(x => new
					{
						Id = x.Id,
						Withdraw = x.WithdrawId,
						Name = x.TermDepositItem.Name,
						Investment = x.Withdraw.Amount,
						InterestRate = x.TermDepositItem.InterestRate,
						Started = x.TermBegin,
						Closed = x.TermEnd,
						Status = x.Status
					});

				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		public async Task<DataTablesResponse> GetOpenDeposits(string userId, DataTablesModel model)
		{
			try
			{
				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var query = context.TermDeposit
						.AsNoTracking()
						.Where(
							x =>
								x.UserId == currentUser &&
								(x.Status == Enums.TermDepositStatus.Active || x.Status == Enums.TermDepositStatus.PendingCancel))
						.OrderByDescending(x => x.TermBegin)
						.Select(x => new
						{
							Id = x.Id,
							Withdraw = x.WithdrawId,
							Name = x.TermDepositItem.Name,
							Investment = x.Withdraw.Amount,
							InterestRate = x.TermDepositItem.InterestRate,
							Started = x.TermBegin,
							Ends = x.TermEnd,
							NextPayout = x.NextPayout,
							Status = x.Status
						});
					return await query.GetDataTableResultNoLockAsync(model);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<DataTablesResponse> GetPayouts(string userId, DataTablesModel model)
		{
			var currentUser = new Guid(userId);
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.TermDepositPayment
					.AsNoTracking()
					.Where(x => x.TermDeposit.UserId == currentUser)
					.OrderByDescending(x => x.Timestamp)
					.Select(x => new
					{
						Id = x.Id,
						TermDepositId = x.TermDepositId,
						Investment = x.TermDeposit.Withdraw.Amount,
						InterestRate = x.TermDeposit.TermDepositItem.InterestRate,
						Payment = x.Amount,
						Type = x.Type,
						Timestamp = x.Timestamp,
						TransactionId = x.TransactionId
					});

				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		public async Task<TermDepositItemModel> GetTermDepositItem(int id)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				return await context.TermDepositItem
					.AsNoTracking()
					.Where(x => x.Id == id && x.IsEnabled)
					.Select(x => new TermDepositItemModel
					{
						Id = x.Id,
						Name = x.Name,
						Description = x.Description,
						Minimum = x.MinInvestment,
						InterestRate = x.InterestRate,
						TermLength = x.TermLength
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
			}
		}

		public async Task<List<TermDepositItemModel>> GetTermDepositItems()
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				return await context.TermDepositItem
					.AsNoTracking()
					.Where(x => x.IsEnabled)
					.OrderBy(x => x.TermLength)
					.Select(x => new TermDepositItemModel
					{
						Id = x.Id,
						Name = x.Name,
						Description = x.Description,
						Minimum = x.MinInvestment,
						InterestRate = x.InterestRate,
						TermLength = x.TermLength
					}).ToListNoLockAsync().ConfigureAwait(false);
			}
		}

		public async Task<List<TermDepositSummaryModel>> GetTermDepositSummary()
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				return await context.TermDepositItem
					.AsNoTracking()
					.Where(x => x.IsEnabled)
					.OrderBy(x => x.TermLength)
					.Select(x => new TermDepositSummaryModel
					{
						Id = x.Id,
						Name = x.Name,
						Description = x.Description,
						Minimum = x.MinInvestment,
						InterestRate = x.InterestRate,
						TermLength = x.TermLength,
						ActiveDeposits = (int?) x.TermDeposits.Count(d => d.Status == Enums.TermDepositStatus.Active) ?? 0,
						ClosedDeposits = (int?) x.TermDeposits.Count(d => d.Status == Enums.TermDepositStatus.Closed) ?? 0,
						TotalInvested = (decimal?) x.TermDeposits.Sum(d => d.Withdraw.Amount) ?? 0,
						ActiveInvested =
							(decimal?) x.TermDeposits.Where(d => d.Status == Enums.TermDepositStatus.Active).Sum(d => d.Withdraw.Amount) ?? 0,
					}).ToListNoLockAsync().ConfigureAwait(false);
			}
		}
	}
}