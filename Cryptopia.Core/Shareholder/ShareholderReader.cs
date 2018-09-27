using Cryptopia.Common.Cache;
using Cryptopia.Common.Currency;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.External;
using Cryptopia.Common.Paytopia;
using Cryptopia.Common.Shareholder;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptopia.Core.Shareholder
{
	public class ShareholderReader : IShareholderReader
	{
		public ICacheService CacheService { get; set; }
		public ICurrencyReader CurrencyReader { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }
		public IExternalApiService ExternalApiService { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<ShareholderModel> GetShareInfo(string userId)
		{
			var result = new ShareholderModel();
			var referralExpenses = await GetReferralExpenses().ConfigureAwait(false);
			var btcPerDollar = await ExternalApiService.ConvertDollarToBTC(1, ConvertDollarToBTCType.USD).ConfigureAwait(false);
			var siteExpenses = await GetSiteExpenses(btcPerDollar).ConfigureAwait(false);

			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var user = await context.Users
					.AsNoTracking()
					.Where(x => x.Id == userId && x.ShareCount > 0)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (user == null)
					return null;
				
				var settings = await context.Settings.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				result.LastPayout = settings.LastSharePayout;
				result.NextPayout = settings.NextSharePayout;
				result.ShareCount = user.ShareCount;
			}

			result.BTCDollar = btcPerDollar;
			result.SiteExpenses = siteExpenses;
			result.TotalExpense = siteExpenses.Sum(x => x.Price);
			result.TotalBTCExpense = siteExpenses.Sum(x => x.BTCPrice);
			result.FeeInfo = await GetTradeFeeInfo(result.LastPayout, result.NextPayout, result.TotalBTCExpense).ConfigureAwait(false);
			result.PaytopiaInfo = await GetPaytopiaFeeInfo(result.LastPayout, result.NextPayout, referralExpenses).ConfigureAwait(false);

			return result;
		}

		public async Task<DataTablesResponse> GetPayoutHistory(string userId, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var currentUser = new Guid(userId);
				var query = context.Transfer
					.AsNoTracking()
					.Where(x => x.TransferType == TransferType.ShareDividend && x.ToUserId == currentUser)
					.Select(transfer => new
					{
						Id = transfer.Id,
						Currency = transfer.Currency.Symbol,
						Amount = transfer.Amount,
						Timestamp = transfer.Timestamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetPaytopiaHistory(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.PaytopiaPayments
					.AsNoTracking()
					.Where(x => x.Status == PaytopiaPaymentStatus.Complete && x.PaytopiaItem.Type != PaytopiaItemType.Shares)
					.Select(payment => new PaytopiaDatatableModel
					{
						Id = payment.Id,
						CurrencyId = payment.PaytopiaItem.CurrencyId,
						Type = payment.PaytopiaItem.Type,
						Amount = payment.Amount,
						Timestamp = payment.Timestamp
					});

				var currencies = await CurrencyReader.GetCurrencies().ConfigureAwait(false);
				var nameMap = currencies.ToDictionary(k => k.CurrencyId, v => v.Name);
				var symbolMap = currencies.ToDictionary(k => k.CurrencyId, v => v.Symbol);
				return await query.GetDataTableResultNoLockAsync(model, (item) =>
				{
					item.Name = nameMap[item.CurrencyId];
					item.Symbol = symbolMap[item.CurrencyId];
				}).ConfigureAwait(false);
			}
		}

		
		private async Task<decimal> GetReferralExpenses()
		{
			var cacheResult = await CacheService.GetOrSetHybridValueAsync(CacheKey.ShareholderReferralExpenses(), TimeSpan.FromHours(1), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					return await context.ReferralInfo
						.AsNoTracking()
						.Where(x => x.Status == ReferralStatus.Active)
						.Select(x => x.ActivityAmount + x.TradeFeeAmount)
						.DefaultIfEmpty(0)
						.SumAsync().ConfigureAwait(false);
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		private async Task<List<SiteExpenseModel>> GetSiteExpenses(decimal btcPerDollar)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ShareholderSiteExpenses(), TimeSpan.FromHours(1), async () =>
			{
				using (var context = DataContextFactory.CreateReadOnlyContext())
				{
					var expenses = await context.SiteExpenses
						.AsNoTracking()
						.Where(x => x.IsEnabled)
						.ToListNoLockAsync().ConfigureAwait(false);

					return expenses.Select(x => new SiteExpenseModel
					{
						Name = x.Name,
						Price = x.Price,
						BTCPrice = Math.Round(btcPerDollar * x.Price, 8)
					}).ToList();
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		private async Task<List<ShareholderFeeInfo>> GetTradeFeeInfo(DateTime lastPayout, DateTime nextPayout, decimal totalBTCExpense)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ShareholderTradeFeeInfo(), TimeSpan.FromHours(1), async () =>
			{
				var baseCurrencies = await CurrencyReader.GetBaseCurrencies().ConfigureAwait(false);
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var feeData = await context.TradeHistory
						.AsNoTracking()
						.Where(x => x.Timestamp > lastPayout && x.Timestamp <= nextPayout)
						.Select(x => new
						{
							BaseCurrency = x.TradePair.Currency2.Symbol,
							Fee = x.Fee
						})
						.GroupBy(x => x.BaseCurrency)
						.Select(x => new
						{
							Symbol = x.Key,
							Total = x.Sum(k => k.Fee)
						}).ToListNoLockAsync().ConfigureAwait(false);

					var results = new List<ShareholderFeeInfo>();
					foreach (var baseCurrency in baseCurrencies.OrderBy(x => x.CurrencyId))
					{
						var fees = feeData.FirstOrDefault(x => x.Symbol == baseCurrency.Symbol);
						var info = new ShareholderFeeInfo
						{
							Name = baseCurrency.Name,
							Symbol = baseCurrency.Symbol,
							TotalFees = (fees?.Total ?? 0) * 2,
							Expenses = baseCurrency.CurrencyId == 1 ? totalBTCExpense : 0
						};
						info.SharePrice = Math.Max(info.TotalFees - info.Expenses, 0) / 20000.0m;
						results.Add(info);
					}
					return results;
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		private async Task<List<ShareholderFeeInfo>> GetPaytopiaFeeInfo(DateTime lastPayout, DateTime nextPayout, decimal referralExpenses)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ShareholderPaytopiaFeeInfo(), TimeSpan.FromHours(6), async () =>
			{
				var currencies = await CurrencyReader.GetCurrencies().ConfigureAwait(false);
				using (var context = DataContextFactory.CreateReadOnlyContext())
				{
					var paytopia = await context.PaytopiaPayments
						.AsNoTracking()
						.Where(x => x.Status == PaytopiaPaymentStatus.Complete && x.PaytopiaItem.Type != PaytopiaItemType.Shares && x.Timestamp > lastPayout && x.Timestamp <= nextPayout)
						.GroupBy(x => x.PaytopiaItem.CurrencyId)
						.Select(x => new
						{
							CurrencyId = x.Key,
							Total = x.Sum(p => p.Amount)
						}).ToListNoLockAsync().ConfigureAwait(false);

					var results = new List<ShareholderFeeInfo>();
					foreach (var item in paytopia)
					{
						var currency = currencies.FirstOrDefault(x => x.CurrencyId == item.CurrencyId);
						if (currency == null)
							continue;

						results.Add(new ShareholderFeeInfo
						{
							Name = currency.Name,
							Symbol = currency.Symbol,
							TotalFees = item.Total,
							Expenses = referralExpenses,
							SharePrice = item.Total > referralExpenses ? ((item.Total - referralExpenses) / 20000.0m) : 0
						});
					}
					return results;
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

	}
}
