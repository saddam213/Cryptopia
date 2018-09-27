using Cryptopia.Common.Paytopia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.Trade;
using Cryptopia.Enums;
using Cryptopia.Common.Currency;
using Cryptopia.Common.Cache;
using Cryptopia.Common.Pool;
using Cryptopia.Common.Lotto;
using Newtonsoft.Json;
using System.Data.Entity;
using Cryptopia.Common.Balance;
using Cryptopia.Base.Extensions;
using Cryptopia.Base;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.Paytopia
{
	public class PaytopiaWriter : IPaytopiaWriter
	{
		private static int _featuredSlotCount = 5;
		public ITradeService TradeService { get; set; }
		public ICacheService CacheService { get; set; }
		public IPoolReader PoolReader { get; set; }
		public ICurrencyReader CurrencyReader { get; set; }
		public ILottoWriter LottoWriter { get; set; }
		public ILottoReader LottoReader { get; set; }
		public IBalanceEstimationService BalanceEstimationService { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }
		public IPoolDataContextFactory DataPoolContextFactory { get; set; }
		public IExchangeDataContextFactory DataExchangeContextFactory { get; set; }

		public async Task<IWriterResult> AdminUpdatePaytopiaPayment(AdminUpdatePaytopiaPaymentModel model)
		{
			var amount = 0m;
			var currencyId = 0;
			var isRefund = false;
			var userId = string.Empty;
			using (var context = DataContextFactory.CreateContext())
			{
				var payment = await context.PaytopiaPayments
					.Include(x => x.PaytopiaItem)
					.Where(x => x.Id == model.PaymentId).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
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
					await context.SaveChangesAsync().ConfigureAwait(false);
					await CacheService.InvalidateAsync(CacheKey.ShareholderPaytopiaFeeInfo());
				}
			}

			if (isRefund)
			{
				var transferResult = await TradeService.CreateTransfer(Constant.SYSTEM_USER_PAYTOPIA.ToString(), new CreateTransferModel
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
					await context.SaveChangesAsync().ConfigureAwait(false);
				}
				return new WriterResult(true, $"Successfully refunded user for payment #{model.PaymentId}");
			}

			return new WriterResult(true, $"Successfully updated payment status for payment #{model.PaymentId} to {model.Status}");
		}

		#region Exchange Listing 

		public async Task<IWriterResult> UpdateExchangeListing(string userId, ExchangeListingModel model)
		{
			var itemId = 0;
			var amount = 0m;
			var currencyId = 0;
			using (var context = DataContextFactory.CreateContext())
			{
				var paytopiaItem = await context.PaytopiaItems
					.Where(x => x.Type == PaytopiaItemType.ExchangeListing && x.IsEnabled)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (paytopiaItem == null)
					return new WriterResult(false, "Paytopia option not found.");

				if (
					paytopiaItem.Payments.Any(x => x.Status == PaytopiaPaymentStatus.Pending && x.ReferenceCode == model.ListingSymbol))
					return new WriterResult(false, $"There is already a pending listing request for this currency.");

				itemId = paytopiaItem.Id;
				amount = paytopiaItem.Price;
				currencyId = paytopiaItem.CurrencyId;
			}

			var transferResult = await TradeService.CreateTransfer(userId, new CreateTransferModel
			{
				Amount = amount,
				CurrencyId = currencyId,
				Receiver = Constant.SYSTEM_USER_PAYTOPIA.ToString(),
				TransferType = TransferType.Paytopia
			});
			if (transferResult.IsError)
				return new WriterResult(false, transferResult.Error);

			using (var context = DataContextFactory.CreateContext())
			{
				var now = DateTime.UtcNow;
				var payment = new Entity.PaytopiaPayment
				{
					Amount = amount,
					TransferId = transferResult.TransferId,
					Status = PaytopiaPaymentStatus.Pending,
					UserId = userId,
					Timestamp = now,
					Begins = now,
					Ends = now,
					IsAnonymous = false,
					PaytopiaItemId = itemId,
					RefundId = 0,
					ReferenceCode = model.ListingSymbol,
					RequestData = JsonConvert.SerializeObject(new
					{
						Name = model.ListingName,
						Symbol = model.ListingSymbol,
						Source = model.ListingSource,
						AlgoType = model.ListingAlgoType.ToString(),
						NetworkType = model.ListingNetworkType.ToString(),
						BlockExplorer = model.ListingBlockExplorer,
						LaunchForum = model.ListingLaunchForum,
						Website = model.ListingWebsite,
						ExtraInfo = model.ListingExtraInfo
					})
				};
				context.PaytopiaPayments.Add(payment);
				await context.SaveChangesAsync().ConfigureAwait(false);
			}

			return new WriterResult(true,
				"Successfully submitted coin listing request, you can track the status of this request in your accounts Paytopia section.");
		}

		#endregion

		#region Pool Listing

		public async Task<IWriterResult> UpdatePoolListing(string userId, PoolListingModel model)
		{
			var itemId = 0;
			var amount = 0m;
			var currencyId = 0;
			var now = DateTime.UtcNow;
			var beginTime = now;
			var expireTime = beginTime.AddDays(30);
			if (model.IsListed)
			{
				var pool = await PoolReader.GetPool(model.ItemId ?? 0);
				if (pool == null)
					return new WriterResult(false, "Pool not found.");

				beginTime = pool.Expires;
				expireTime = beginTime.AddDays(30);
			}

			using (var context = DataContextFactory.CreateContext())
			{
				var paytopiaItem = await context.PaytopiaItems
					.Where(x => x.Type == PaytopiaItemType.PoolListing && x.IsEnabled)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (paytopiaItem == null)
					return new WriterResult(false, "Paytopia option not found.");

				if (
					paytopiaItem.Payments.Any(
						x =>
							x.Status == PaytopiaPaymentStatus.Pending && x.ReferenceCode == $"{model.ListingSymbol}-{model.ListingAlgoType}"))
					return new WriterResult(false, $"There is already a pending listing request for this pool.");

				itemId = paytopiaItem.Id;
				amount = paytopiaItem.Price;
				currencyId = paytopiaItem.CurrencyId;
			}

			var transferResult = await TradeService.CreateTransfer(userId, new CreateTransferModel
			{
				Amount = amount,
				CurrencyId = currencyId,
				Receiver = Constant.SYSTEM_USER_PAYTOPIA.ToString(),
				TransferType = TransferType.Paytopia
			});
			if (transferResult.IsError)
				return new WriterResult(false, transferResult.Error);

			using (var context = DataContextFactory.CreateContext())
			{
				var payment = new Entity.PaytopiaPayment
				{
					Amount = amount,
					TransferId = transferResult.TransferId,
					Status = model.IsListed ? PaytopiaPaymentStatus.Complete : PaytopiaPaymentStatus.Pending,
					UserId = userId,
					Timestamp = now,
					Begins = beginTime,
					Ends = expireTime,
					IsAnonymous = false,
					PaytopiaItemId = itemId,
					RefundId = 0,
					ReferenceCode = model.IsListed ? string.Empty : $"{model.ListingSymbol}-{model.ListingAlgoType}",
					ReferenceId = model.IsListed ? model.ItemId.Value : 0,
					RequestData = model.IsListed
						? string.Empty
						: JsonConvert.SerializeObject(new
						{
							Name = model.ListingName,
							Symbol = model.ListingSymbol,
							Source = model.ListingSource,
							AlgoType = model.ListingAlgoType.ToString(),
							BlockExplorer = model.ListingBlockExplorer,
							LaunchForum = model.ListingLaunchForum,
							NetworkType = model.ListingNetworkType.ToString(),
							Website = model.ListingWebsite,
							ExtraInfo = model.ListingExtraInfo
						})
				};
				context.PaytopiaPayments.Add(payment);
				await context.SaveChangesAsync().ConfigureAwait(false);
			}

			if (model.IsListed)
			{
				await CacheService.InvalidateAsync(CacheKey.ShareholderPaytopiaFeeInfo());
				using (var context = DataPoolContextFactory.CreateContext())
				{
					var updatePool = await context.Pool.FirstOrDefaultNoLockAsync(x => x.Id == model.ItemId.Value);
					if (updatePool == null)
						return new WriterResult(false, "Pool not found.");

					var newExpireTime = updatePool.Expires.AddDays(30);
					updatePool.Expires = newExpireTime;
					if (updatePool.Status == PoolStatus.Expired || updatePool.Status == PoolStatus.Expiring)
					{
						updatePool.Status = PoolStatus.OK;
					}
					await context.SaveChangesAsync().ConfigureAwait(false);
					await
						CacheService.InvalidateAsync(CacheKey.Pool(updatePool.Id), CacheKey.Pools(), CacheKey.MineshaftSummary(),
							CacheKey.MineshaftSummary(updatePool.AlgoType));
					return new WriterResult(true,
						$"Successfully extended the {updatePool.Symbol} {updatePool.AlgoType} pool subscription until {newExpireTime.ToString("dd/MM/yyyy")}");
				}
			}
			return new WriterResult(true,
				"Successfully submitted pool listing request, you can track the status of this request in your accounts Paytopia section.");
		}

		#endregion

		#region Combo Listing 

		public async Task<IWriterResult> UpdateComboListing(string userId, ExchangeListingModel model)
		{
			var itemId = 0;
			var amount = 0m;
			var currencyId = 0;
			using (var context = DataContextFactory.CreateContext())
			{
				var paytopiaItem = await context.PaytopiaItems
					.Where(x => x.Type == PaytopiaItemType.ComboListing && x.IsEnabled)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (paytopiaItem == null)
					return new WriterResult(false, "Paytopia option not found.");

				if (
					paytopiaItem.Payments.Any(x => x.Status == PaytopiaPaymentStatus.Pending && x.ReferenceCode == model.ListingSymbol))
					return new WriterResult(false, $"There is already a pending listing request for this currency.");

				itemId = paytopiaItem.Id;
				amount = paytopiaItem.Price;
				currencyId = paytopiaItem.CurrencyId;
			}

			var transferResult = await TradeService.CreateTransfer(userId, new CreateTransferModel
			{
				Amount = amount,
				CurrencyId = currencyId,
				Receiver = Constant.SYSTEM_USER_PAYTOPIA.ToString(),
				TransferType = TransferType.Paytopia
			});
			if (transferResult.IsError)
				return new WriterResult(false, transferResult.Error);

			using (var context = DataContextFactory.CreateContext())
			{
				var now = DateTime.UtcNow;
				var payment = new Entity.PaytopiaPayment
				{
					Amount = amount,
					TransferId = transferResult.TransferId,
					Status = PaytopiaPaymentStatus.Pending,
					UserId = userId,
					Timestamp = now,
					Begins = now,
					Ends = now,
					IsAnonymous = false,
					PaytopiaItemId = itemId,
					RefundId = 0,
					ReferenceCode = model.ListingSymbol,
					RequestData = JsonConvert.SerializeObject(new
					{
						Name = model.ListingName,
						Symbol = model.ListingSymbol,
						Source = model.ListingSource,
						AlgoType = model.ListingAlgoType.ToString(),
						NetworkType = model.ListingNetworkType.ToString(),
						BlockExplorer = model.ListingBlockExplorer,
						LaunchForum = model.ListingLaunchForum,
						Website = model.ListingWebsite,
						ExtraInfo = model.ListingExtraInfo
					})
				};
				context.PaytopiaPayments.Add(payment);
				await context.SaveChangesAsync().ConfigureAwait(false);
			}

			return new WriterResult(true,
				"Successfully submitted listing request, you can track the status of this request in your accounts Paytopia section.");
		}

		#endregion

		#region Lotto Slot

		public async Task<IWriterResult> UpdateLottoSlot(string userId, LottoSlotModel model)
		{
			var itemId = 0;
			var amount = 0m;
			var currencyId = 0;
			var currency = await CurrencyReader.GetCurrency(model.ItemId);
			if (currency == null)
				return new WriterResult(false, "Currency not found.");

			var existingLotto = await LottoReader.GetLottoItems(string.Empty);
			if (existingLotto.Any(x => x.CurrencyId == currency.CurrencyId && x.Name == model.Name))
				return new WriterResult(false,
					$"Lotto game with name '{model.Name}' already exists for {currency.Symbol}, please choose a new name for this game.");

			using (var context = DataContextFactory.CreateContext())
			{
				var paytopiaItem = await context.PaytopiaItems
					.Where(x => x.Type == PaytopiaItemType.LottoSlot && x.IsEnabled)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (paytopiaItem == null)
					return new WriterResult(false, "Paytopia option not found.");

				itemId = paytopiaItem.Id;
				amount = paytopiaItem.Price;
				currencyId = paytopiaItem.CurrencyId;
			}

			var transferResult = await TradeService.CreateTransfer(userId, new CreateTransferModel
			{
				Amount = amount,
				CurrencyId = currencyId,
				Receiver = Constant.SYSTEM_USER_PAYTOPIA.ToString(),
				TransferType = TransferType.Paytopia
			});
			if (transferResult.IsError)
				return new WriterResult(false, transferResult.Error);

			using (var context = DataContextFactory.CreateContext())
			{
				var payment = new Entity.PaytopiaPayment
				{
					Amount = amount,
					TransferId = transferResult.TransferId,
					Status = PaytopiaPaymentStatus.Complete,
					UserId = userId,
					Timestamp = DateTime.UtcNow,
					IsAnonymous = false,
					Begins = DateTime.UtcNow,
					Ends = DateTime.UtcNow.AddDays(30),
					PaytopiaItemId = itemId,
					RefundId = 0,
					RequestData = JsonConvert.SerializeObject(new
					{
						CurrencyId = currency.CurrencyId,
						Symbol = currency.Symbol,
						Name = model.LottoName,
						Description = model.LottoDescription,
						TicketPrice = model.LottoPrice
					})
				};
				context.PaytopiaPayments.Add(payment);
				await context.SaveChangesAsync().ConfigureAwait(false);
				await CacheService.InvalidateAsync(CacheKey.ShareholderPaytopiaFeeInfo());
			}

			// Create the lotto item
			var result = await LottoWriter.CreateLottoItem(new CreateLottoItemModel
			{
				CurrencyId = currency.CurrencyId,
				Description = model.LottoDescription,
				Name = model.LottoName,
				Fee = 0,
				LottoType = LottoType.RecurringExpire,
				Prizes = 3,
				Rate = model.LottoPrice,
				Status = LottoItemStatus.Active,
				Hours = 24,
				NextDraw = DateTime.UtcNow.AddHours(24),
				Expires = DateTime.UtcNow.AddDays(30)
			});
			if (!result.Success)
				return new WriterResult(false, "An error occurred processing your request, please contact Cryptopia support");

			return new WriterResult(true,
				$"Successfully purchased new Lotto game '{currency.Name}', First draw at {DateTime.UtcNow.AddHours(24)}");
		}

		#endregion

		#region Reward Slot

		public async Task<IWriterResult> UpdateRewardSlot(string userId, RewardSlotModel model)
		{
			var currency = await CurrencyReader.GetCurrency(model.ItemId ?? 0);
			if (currency == null)
				return new WriterResult(false, "Currency not found.");

			var itemId = 0;
			var amount = 0m;
			var currencyId = 0;
			var isExtended = false;
			var beginTime = DateTime.UtcNow;
			using (var context = DataContextFactory.CreateContext())
			{
				var paytopiaItem = await context.PaytopiaItems
					.Where(x => x.Type == PaytopiaItemType.RewardSlot && x.IsEnabled)
					.Include(x => x.Payments)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (paytopiaItem == null)
					return new WriterResult(false, "Paytopia option not found.");

				itemId = paytopiaItem.Id;
				amount = paytopiaItem.Price;
				currencyId = paytopiaItem.CurrencyId;
				var lastPayment = paytopiaItem.Payments
					.Where(x => x.ReferenceId == currency.CurrencyId)
					.OrderByDescending(x => x.Ends)
					.FirstOrDefault();
				if (lastPayment != null && lastPayment.Ends > beginTime)
				{
					beginTime = lastPayment.Ends;
					isExtended = true;
				}
			}

			var transferResult = await TradeService.CreateTransfer(userId, new CreateTransferModel
			{
				Amount = amount,
				CurrencyId = currencyId,
				Receiver = Constant.SYSTEM_USER_PAYTOPIA.ToString(),
				TransferType = TransferType.Paytopia
			});
			if (transferResult.IsError)
				return new WriterResult(false, transferResult.Error);

			var expireTime = beginTime.AddDays(30);
			using (var context = DataContextFactory.CreateContext())
			{
				var payment = new Entity.PaytopiaPayment
				{
					Amount = amount,
					TransferId = transferResult.TransferId,
					Status = PaytopiaPaymentStatus.Complete,
					UserId = userId,
					Timestamp = DateTime.UtcNow,
					IsAnonymous = false,
					Begins = beginTime,
					Ends = expireTime,
					PaytopiaItemId = itemId,
					RefundId = 0,
					ReferenceId = currency.CurrencyId
				};
				context.PaytopiaPayments.Add(payment);
				await context.SaveChangesAsync().ConfigureAwait(false);
				await CacheService.InvalidateAsync(CacheKey.ShareholderPaytopiaFeeInfo());
			}

			using (var context = DataExchangeContextFactory.CreateContext())
			{
				var updateCurrency = await context.Currency.FirstOrDefaultNoLockAsync(x => x.Id == model.ItemId);
				if (updateCurrency == null)
					return new WriterResult(false, "Currency not found.");

				updateCurrency.RewardsExpires = expireTime;
				await context.SaveChangesAsync().ConfigureAwait(false);
				await CacheService.InvalidateAsync(CacheKey.Currencies());

				var message = isExtended
					? $"Successfully extended reward slot, slot expires on {expireTime.ToString("dd/MM/yyyy")}"
					: $"Successfully processed payment, {currency.Status} reward slot is now active until {expireTime.ToString("dd/MM/yyyy")}";
				return new WriterResult(true, message);
			}
		}

		#endregion

		#region Tipping Slot

		public async Task<IWriterResult> UpdateTipSlot(string userId, TipSlotModel model)
		{
			var currency = await CurrencyReader.GetCurrency(model.ItemId ?? 0);
			if (currency == null)
				return new WriterResult(false, "Currency not found.");

			var itemId = 0;
			var amount = 0m;
			var currencyId = 0;
			var isExtended = false;
			var beginTime = DateTime.UtcNow;
			using (var context = DataContextFactory.CreateContext())
			{
				var paytopiaItem = await context.PaytopiaItems
					.Where(x => x.Type == PaytopiaItemType.TipSlot && x.IsEnabled)
					.Include(x => x.Payments)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (paytopiaItem == null)
					return new WriterResult(false, "Paytopia option not found.");

				itemId = paytopiaItem.Id;
				amount = paytopiaItem.Price;
				currencyId = paytopiaItem.CurrencyId;
				var lastPayment = paytopiaItem.Payments
					.Where(x => x.ReferenceId == currency.CurrencyId)
					.OrderByDescending(x => x.Ends)
					.FirstOrDefault();
				if (lastPayment != null && lastPayment.Ends > beginTime)
				{
					beginTime = lastPayment.Ends;
					isExtended = true;
				}
			}

			var transferResult = await TradeService.CreateTransfer(userId, new CreateTransferModel
			{
				Amount = amount,
				CurrencyId = currencyId,
				Receiver = Constant.SYSTEM_USER_PAYTOPIA.ToString(),
				TransferType = TransferType.Paytopia
			});
			if (transferResult.IsError)
				return new WriterResult(false, transferResult.Error);

			var expireTime = beginTime.AddDays(30);
			using (var context = DataContextFactory.CreateContext())
			{
				var payment = new Entity.PaytopiaPayment
				{
					Amount = amount,
					TransferId = transferResult.TransferId,
					Status = PaytopiaPaymentStatus.Complete,
					UserId = userId,
					Timestamp = DateTime.UtcNow,
					IsAnonymous = false,
					Begins = beginTime,
					Ends = expireTime,
					PaytopiaItemId = itemId,
					RefundId = 0,
					ReferenceId = currency.CurrencyId
				};
				context.PaytopiaPayments.Add(payment);
				await context.SaveChangesAsync().ConfigureAwait(false);
				await CacheService.InvalidateAsync(CacheKey.ShareholderPaytopiaFeeInfo());
			}

			using (var context = DataExchangeContextFactory.CreateContext())
			{
				var updateCurrency = await context.Currency.FirstOrDefaultNoLockAsync(x => x.Id == model.ItemId);
				if (updateCurrency == null)
					return new WriterResult(false, "Currency not found.");

				updateCurrency.MinTip = await BalanceEstimationService.GetEstimatedAmount(0.00000500m, currency.CurrencyId);
				updateCurrency.TippingExpires = expireTime;
				await context.SaveChangesAsync().ConfigureAwait(false);
				await CacheService.InvalidateAsync(CacheKey.Currencies());

				var message = isExtended
					? $"Successfully extended tipping slot, slot expires on {expireTime.ToString("dd/MM/yyyy")}"
					: $"Successfully processed payment, {currency.Status} tipping is now active until {expireTime.ToString("dd/MM/yyyy")}";
				return new WriterResult(true, message);
			}
		}

		#endregion

		#region Featured Currency

		public async Task<IWriterResult> UpdateFeaturedCurrencySlot(string userId, FeaturedSlotModel model)
		{
			var itemId = 0;
			var amount = 0m;
			var currencyId = 0;
			TimeSlotInfo nextFreeSlot = null;
			var currency = await CurrencyReader.GetCurrency(model.ItemId.Value);
			if (currency == null)
				return new WriterResult(false, "Currency not found.");

			using (var context = DataContextFactory.CreateContext())
			{
				var paytopiaItem = await context.PaytopiaItems
					.Include(x => x.Payments)
					.Where(x => x.Type == PaytopiaItemType.FeaturedCurrency && x.IsEnabled)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (paytopiaItem == null)
					return new WriterResult(false);

				nextFreeSlot = GetNextFreeSlot(model.ItemId.Value, paytopiaItem.Payments);
				itemId = paytopiaItem.Id;
				amount = paytopiaItem.Price;
				currencyId = paytopiaItem.CurrencyId;
			}

			if (nextFreeSlot == null)
				return new WriterResult(false, "Unable to locate free time slot for featured pool.");

			var transferResult = await TradeService.CreateTransfer(userId, new CreateTransferModel
			{
				Amount = amount,
				CurrencyId = currencyId,
				Receiver = Constant.SYSTEM_USER_PAYTOPIA.ToString(),
				TransferType = TransferType.Paytopia
			});
			if (transferResult.IsError)
				return new WriterResult(false, transferResult.Error);

			using (var context = DataContextFactory.CreateContext())
			{
				var payment = new Entity.PaytopiaPayment
				{
					Amount = amount,
					TransferId = transferResult.TransferId,
					Status = PaytopiaPaymentStatus.Complete,
					UserId = userId,
					Timestamp = DateTime.UtcNow,
					IsAnonymous = model.IsAnonymous,
					Begins = nextFreeSlot.Begin,
					Ends = nextFreeSlot.End,
					PaytopiaItemId = itemId,
					RefundId = 0,
					ReferenceId = model.ItemId.Value
				};
				context.PaytopiaPayments.Add(payment);
				await context.SaveChangesAsync().ConfigureAwait(false);
				await CacheService.InvalidateAsync(CacheKey.ShareholderPaytopiaFeeInfo());
			}

			if (nextFreeSlot.Begin.WeekOfYear() == DateTime.UtcNow.WeekOfYear())
			{
				using (var context = DataExchangeContextFactory.CreateContext())
				{
					var featuredCurrency =
						await context.Currency.Where(x => x.Id == model.ItemId.Value).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (featuredCurrency == null)
						return new WriterResult(false);

					featuredCurrency.FeaturedExpires = nextFreeSlot.End;
					await context.SaveChangesAsync().ConfigureAwait(false);
					await CacheService.InvalidateAsync(CacheKey.Currencies(), CacheKey.CurrencyInfo(), CacheKey.ExchangeSummary());
				}
			}
			return new WriterResult(true,
				$"Successfully purchased {currency.Symbol} featured time slot {nextFreeSlot.Begin.ToString("dd/MM/yyyy")} - {nextFreeSlot.End.ToString("dd/MM/yyyy")}");
		}

		#endregion

		#region Featured Pool

		public async Task<IWriterResult> UpdateFeaturedPoolSlot(string userId, FeaturedSlotModel model)
		{
			var itemId = 0;
			var amount = 0m;
			var currencyId = 0;
			TimeSlotInfo nextFreeSlot = null;
			var pool = await PoolReader.GetPool(model.ItemId.Value);
			if (pool == null)
				return new WriterResult(false, "Pool not found.");

			var pools = await PoolReader.GetPools();
			using (var context = DataContextFactory.CreateContext())
			{
				var paytopiaItem = await context.PaytopiaItems
					.Include(x => x.Payments)
					.Where(x => x.Type == PaytopiaItemType.FeaturedPool && x.IsEnabled)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (paytopiaItem == null)
					return new WriterResult(false);

				nextFreeSlot = GetNextFreeSlot(model.ItemId.Value, paytopiaItem.Payments);
				itemId = paytopiaItem.Id;
				amount = paytopiaItem.Price;
				currencyId = paytopiaItem.CurrencyId;
			}

			if (nextFreeSlot == null)
				return new WriterResult(false, "Unable to locate free time slot for featured pool.");

			var transferResult = await TradeService.CreateTransfer(userId, new CreateTransferModel
			{
				Amount = amount,
				CurrencyId = currencyId,
				Receiver = Constant.SYSTEM_USER_PAYTOPIA.ToString(),
				TransferType = TransferType.Paytopia
			});
			if (transferResult.IsError)
				return new WriterResult(false, transferResult.Error);

			using (var context = DataContextFactory.CreateContext())
			{
				var payment = new Entity.PaytopiaPayment
				{
					Amount = amount,
					TransferId = transferResult.TransferId,
					Status = PaytopiaPaymentStatus.Complete,
					UserId = userId,
					Timestamp = DateTime.UtcNow,
					IsAnonymous = model.IsAnonymous,
					Begins = nextFreeSlot.Begin,
					Ends = nextFreeSlot.End,
					PaytopiaItemId = itemId,
					RefundId = 0,
					ReferenceId = model.ItemId.Value
				};
				context.PaytopiaPayments.Add(payment);
				await context.SaveChangesAsync().ConfigureAwait(false);
				await CacheService.InvalidateAsync(CacheKey.ShareholderPaytopiaFeeInfo());
			}

			if (nextFreeSlot.Begin.WeekOfYear() == DateTime.UtcNow.WeekOfYear())
			{
				using (var context = DataPoolContextFactory.CreateContext())
				{
					var featuredPool =
						await context.Pool.Where(x => x.Id == model.ItemId.Value).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (featuredPool == null)
						return new WriterResult(false);

					featuredPool.FeaturedExpires = nextFreeSlot.End;
					await context.SaveChangesAsync().ConfigureAwait(false);
					await
						CacheService.InvalidateAsync(CacheKey.Pool(model.ItemId.Value), CacheKey.Pools(), CacheKey.MineshaftSummary());
				}
			}
			return new WriterResult(true,
				$"Successfully purchased {pool.Symbol} featured time slot {nextFreeSlot.Begin.ToString("dd/MM/yyyy")} - {nextFreeSlot.End.ToString("dd/MM/yyyy")}");
		}

		#endregion

		#region Shares

		public async Task<IWriterResult> UpdateShares(string userId, SharesModel model)
		{
			var itemId = 0;
			var amount = 0m;
			var currencyId = 0;
			Entity.ApplicationUser user = null;
			using (var context = DataContextFactory.CreateContext())
			{
				user = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == userId);
				if (user == null)
					return new WriterResult(false, "User not found.");

				var paytopiaItem = await context.PaytopiaItems
					.Include(x => x.Payments)
					.Where(x => x.Type == PaytopiaItemType.Shares && x.IsEnabled)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (paytopiaItem == null)
					return new WriterResult(false);

				itemId = paytopiaItem.Id;
				amount = paytopiaItem.Price*model.Count;
				currencyId = paytopiaItem.CurrencyId;
			}

			var transferResult = await TradeService.CreateTransfer(userId, new CreateTransferModel
			{
				Amount = amount,
				CurrencyId = currencyId,
				Receiver = Constant.SYSTEM_USER_PAYTOPIA.ToString(),
				TransferType = TransferType.Paytopia
			});
			if (transferResult.IsError)
				return new WriterResult(false, transferResult.Error);

			using (var context = DataContextFactory.CreateContext())
			{
				var payment = new Entity.PaytopiaPayment
				{
					Amount = amount,
					TransferId = transferResult.TransferId,
					Status = PaytopiaPaymentStatus.Pending,
					UserId = userId,
					Timestamp = DateTime.UtcNow,
					IsAnonymous = true,
					Begins = DateTime.UtcNow,
					Ends = DateTime.UtcNow,
					PaytopiaItemId = itemId,
					RefundId = 0,
					ReferenceId = model.Count,
					RequestData = JsonConvert.SerializeObject(new
					{
						UserName = user.UserName,
						IsShareholder = user.ShareCount > 0,
						ShareCount = model.Count,
						FirstName = model.FirstName,
						MiddleName = model.MiddleName,
						LastName = model.LastName,
						Street = model.Street,
						City = model.City,
						PostCode = model.PostCode,
						Country = model.Country,
						Email = model.Email,
						Phone = model.Phone
					})
				};
				context.PaytopiaPayments.Add(payment);
				await context.SaveChangesAsync().ConfigureAwait(false);
			}

			return new WriterResult(true,
				"Successfully submitted share purchase request, you can track the status of this request in your accounts Paytopia section.");
		}

		#endregion

		#region TwoFactor

		public async Task<IWriterResult> UpdateTwoFactor(string userId, TwoFactorModel model)
		{
			var itemId = 0;
			var amount = 0m;
			var currencyId = 0;
			Entity.ApplicationUser user = null;
			using (var context = DataContextFactory.CreateContext())
			{
				user = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == userId);
				if (user == null)
					return new WriterResult(false, "User not found.");

				var paytopiaItem = await context.PaytopiaItems
					.Include(x => x.Payments)
					.Where(x => x.Type == PaytopiaItemType.TwoFactor && x.IsEnabled)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (paytopiaItem == null)
					return new WriterResult(false);

				itemId = paytopiaItem.Id;
				amount = paytopiaItem.Price;
				currencyId = paytopiaItem.CurrencyId;
			}

			var transferResult = await TradeService.CreateTransfer(userId, new CreateTransferModel
			{
				Amount = amount,
				CurrencyId = currencyId,
				Receiver = Constant.SYSTEM_USER_PAYTOPIA.ToString(),
				TransferType = TransferType.Paytopia
			});
			if (transferResult.IsError)
				return new WriterResult(false, transferResult.Error);

			using (var context = DataContextFactory.CreateContext())
			{
				var payment = new Entity.PaytopiaPayment
				{
					Amount = amount,
					TransferId = transferResult.TransferId,
					Status = PaytopiaPaymentStatus.Pending,
					UserId = userId,
					Timestamp = DateTime.UtcNow,
					IsAnonymous = true,
					Begins = DateTime.UtcNow,
					Ends = DateTime.UtcNow,
					PaytopiaItemId = itemId,
					RefundId = 0,
					RequestData = JsonConvert.SerializeObject(new
					{
						UserName = user.UserName,
						Recipient = model.Recipient,
						Street = model.Street,
						City = model.City,
						PostCode = model.PostCode,
						Country = model.Country,
					})
				};
				context.PaytopiaPayments.Add(payment);
				await context.SaveChangesAsync().ConfigureAwait(false);
			}

			return new WriterResult(true, "Successfully submitted Cryptopia Authentication Device purchase request, you can track the status of this request in your accounts Paytopia section.");
		}

		#endregion

		#region Avatar

		public async Task<IWriterResult> UpdateAvatar(string userId, AvatarModel model)
		{
			var itemId = 0;
			var amount = 0m;
			var currencyId = 0;
			using (var context = DataContextFactory.CreateContext())
			{
				var paytopiaItem = await context.PaytopiaItems
					.Include(x => x.Payments)
					.Where(x => x.Type == PaytopiaItemType.Avatar && x.IsEnabled)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (paytopiaItem == null)
					return new WriterResult(false);

				itemId = paytopiaItem.Id;
				amount = paytopiaItem.Price;
				currencyId = paytopiaItem.CurrencyId;
			}

			var transferResult = await TradeService.CreateTransfer(userId, new CreateTransferModel
			{
				Amount = amount,
				CurrencyId = currencyId,
				Receiver = Constant.SYSTEM_USER_PAYTOPIA.ToString(),
				TransferType = TransferType.Paytopia
			});
			if (transferResult.IsError)
				return new WriterResult(false, transferResult.Error);

			using (var context = DataContextFactory.CreateContext())
			{
				var payment = new Entity.PaytopiaPayment
				{
					Amount = amount,
					TransferId = transferResult.TransferId,
					Status = PaytopiaPaymentStatus.Complete,
					UserId = userId,
					Timestamp = DateTime.UtcNow,
					IsAnonymous = false,
					Begins = DateTime.UtcNow,
					Ends = DateTime.UtcNow,
					PaytopiaItemId = itemId,
					RefundId = 0,
					ReferenceId = 0
				};
				context.PaytopiaPayments.Add(payment);
				await context.SaveChangesAsync().ConfigureAwait(false);
				await CacheService.InvalidateAsync(CacheKey.ShareholderPaytopiaFeeInfo());
			}

			// Save file to disk

			return new WriterResult(true, $"Successfully purchased animated avatar.");
		}

		#endregion

		#region Emoticon

		public async Task<IWriterResult> UpdateEmoticon(string userId, EmoticonModel model)
		{
			var itemId = 0;
			var amount = 0m;
			var currencyId = 0;
			using (var context = DataContextFactory.CreateContext())
			{
				var paytopiaItem = await context.PaytopiaItems
					.Include(x => x.Payments)
					.Where(x => x.Type == PaytopiaItemType.Emoticon && x.IsEnabled)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (paytopiaItem == null)
					return new WriterResult(false);

				itemId = paytopiaItem.Id;
				amount = paytopiaItem.Price;
				currencyId = paytopiaItem.CurrencyId;
			}

			var transferResult = await TradeService.CreateTransfer(userId, new CreateTransferModel
			{
				Amount = amount,
				CurrencyId = currencyId,
				Receiver = Constant.SYSTEM_USER_PAYTOPIA.ToString(),
				TransferType = TransferType.Paytopia
			});
			if (transferResult.IsError)
				return new WriterResult(false, transferResult.Error);

			using (var context = DataContextFactory.CreateContext())
			{
				var payment = new Entity.PaytopiaPayment
				{
					Amount = amount,
					TransferId = transferResult.TransferId,
					Status = PaytopiaPaymentStatus.Complete,
					UserId = userId,
					Timestamp = DateTime.UtcNow,
					IsAnonymous = false,
					Begins = DateTime.UtcNow,
					Ends = DateTime.UtcNow,
					PaytopiaItemId = itemId,
					RefundId = 0,
					ReferenceId = 0
				};
				context.PaytopiaPayments.Add(payment);
				await context.SaveChangesAsync().ConfigureAwait(false);
				await CacheService.InvalidateAsync(CacheKey.ShareholderPaytopiaFeeInfo());
			}

			// Save emoticon to disk

			return new WriterResult(true, $"Successfully purchased Trollbox emoticon.");
		}

		#endregion

		#region Flair

		public async Task<IWriterResult> UpdateFlair(string userId, FlairModel model)
		{
			var itemId = 0;
			var amount = 0m;
			var currencyId = 0;
			using (var context = DataContextFactory.CreateContext())
			{
				var paytopiaItem = await context.PaytopiaItems
					.Include(x => x.Payments)
					.Where(x => x.Type == PaytopiaItemType.Flair && x.IsEnabled)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (paytopiaItem == null)
					return new WriterResult(false);

				itemId = paytopiaItem.Id;
				amount = paytopiaItem.Price;
				currencyId = paytopiaItem.CurrencyId;
			}

			var transferResult = await TradeService.CreateTransfer(userId, new CreateTransferModel
			{
				Amount = amount,
				CurrencyId = currencyId,
				Receiver = Constant.SYSTEM_USER_PAYTOPIA.ToString(),
				TransferType = TransferType.Paytopia
			});
			if (transferResult.IsError)
				return new WriterResult(false, transferResult.Error);

			using (var context = DataContextFactory.CreateContext())
			{
				var payment = new Entity.PaytopiaPayment
				{
					Amount = amount,
					TransferId = transferResult.TransferId,
					Status = PaytopiaPaymentStatus.Complete,
					UserId = userId,
					Timestamp = DateTime.UtcNow,
					IsAnonymous = false,
					Begins = DateTime.UtcNow,
					Ends = DateTime.UtcNow,
					PaytopiaItemId = itemId,
					RefundId = 0,
					ReferenceId = 0
				};
				context.PaytopiaPayments.Add(payment);
				await context.SaveChangesAsync().ConfigureAwait(false);
				await CacheService.InvalidateAsync(CacheKey.ShareholderPaytopiaFeeInfo());
			}

			// Save flair to database

			return new WriterResult(true, $"Successfully purchased Trollbox flair.");
		}

		#endregion

		public static TimeSlotInfo GetNextFreeSlot(int itemId, IEnumerable<Entity.PaytopiaPayment> payments)
		{
			var now = DateTime.UtcNow.Date;
			if (!payments.IsNullOrEmpty())
			{
				// Get all future slots
				var slots = payments.Where(x => x.Begins > now)
					.GroupBy(x => new {Year = x.Begins.Year, Week = x.Begins.WeekOfYear()})
					.OrderBy(x => x.Key.Year)
					.ThenBy(x => x.Key.Week)
					.ToList();
				if (slots.Any())
				{
					foreach (var slot in slots)
					{
						if (slot.Count() < _featuredSlotCount && !slot.Any(x => x.ReferenceId == itemId))
						{
							var item = slot.FirstOrDefault();
							return new TimeSlotInfo
							{
								Begin = item.Begins,
								End = item.Ends
							};
						}
					}

					// All existing future slots are full, create new future slot
					var lastSlot = slots.SelectMany(x => x).Max(x => x.Ends);
					return new TimeSlotInfo
					{
						Begin = lastSlot,
						End = lastSlot.AddDays(7)
					};
				}
			}


			// There are no future slots, create one for today if its Monday else next Monday
			return new TimeSlotInfo
			{
				Begin = now.DayOfWeek == DayOfWeek.Monday
					? now.Date
					: now.StartOfWeek(DayOfWeek.Monday).Date.AddDays(7),
				End = now.DayOfWeek == DayOfWeek.Monday
					? now.Date.AddDays(7)
					: now.StartOfWeek(DayOfWeek.Monday).Date.AddDays(14)
			};
		}


		public class TimeSlotInfo
		{
			public DateTime End { get; set; }
			public DateTime Begin { get; set; }
		}
	}
}
