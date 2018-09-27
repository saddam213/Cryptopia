using Cryptopia.Common.Cache;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.TradePair;
using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.TradeNotification;
using Cryptopia.Base;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Core.TradePair
{
	public class TradePairReader : ITradePairReader
	{
		public ICacheService CacheService { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<TradePairModel> GetTradePair(int tradePairId, bool includeClosed = false)
		{
			return await GetTradePair(x => x.TradePairId == tradePairId, includeClosed).ConfigureAwait(false);
		}

		public async Task<TradePairModel> GetTradePair(string market, bool includeClosed = false)
		{
			return await GetTradePair(x => x.Market == market?.ToUpper(), includeClosed).ConfigureAwait(false);
		}

		public async Task<TradePairModel> GetTradePair(string currency, string baseCurrency, bool includeClosed = false)
		{
			return await GetTradePair(x => x.Symbol == currency && x.BaseSymbol == baseCurrency, includeClosed).ConfigureAwait(false);
		}

		public async Task<TradePairModel> GetTradePair(int currency, int baseCurrency, bool includeClosed = false)
		{
			return await GetTradePair(x => x.CurrencyId == currency && x.BaseCurrencyId == baseCurrency, includeClosed).ConfigureAwait(false);
		}

		public async Task<List<TradePairModel>> GetTradePairs(bool includeClosed = false)
		{
			return includeClosed
				? await GetOrCacheAllTradePairs().ConfigureAwait(false)
				: await GetOrCacheTradePairs().ConfigureAwait(false);
		}

		private async Task<TradePairModel> GetTradePair(Func<TradePairModel, bool> selector, bool includeClosed = false)
		{
			var tradePairs = await GetTradePairs(includeClosed).ConfigureAwait(false);
			if (tradePairs != null)
				return tradePairs.FirstOrDefault(selector);

			return null;
		}

		private async Task<List<TradePairModel>> GetOrCacheTradePairs()
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.TradePairs(), TimeSpan.FromMinutes(5), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var summary = await context.TradePair
						.AsNoTracking()
						.Where(t => t.Status != TradePairStatus.Closed)
						.Select(t => new TradePairModel
						{
							TradePairId = t.Id,
							Market = string.Concat(t.Currency1.Symbol, "_", t.Currency2.Symbol),
							CurrencyId = t.CurrencyId1,
							Symbol = t.Currency1.Symbol,
							Name = t.Currency1.Name,
							BaseCurrencyId = t.CurrencyId2,
							BaseSymbol = t.Currency2.Symbol,
							BaseName = t.Currency2.Name,
							BaseFee = t.Currency2.TradeFee,
							BaseMinTrade = t.Currency2.MinBaseTrade,
							Change = t.Change,
							LastTrade = t.LastTrade,
							Status = t.Status,
							StatusMessage = t.StatusMessage
						}).ToListNoLockAsync().ConfigureAwait(false);
					return summary;
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		private async Task<List<TradePairModel>> GetOrCacheAllTradePairs()
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.AllTradePairs(), TimeSpan.FromMinutes(5), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var summary = await context.TradePair
						.AsNoTracking()
						.Select(t => new TradePairModel
						{
							TradePairId = t.Id,
							Market = string.Concat(t.Currency1.Symbol, "_", t.Currency2.Symbol),
							CurrencyId = t.CurrencyId1,
							Symbol = t.Currency1.Symbol,
							Name = t.Currency1.Name,
							BaseCurrencyId = t.CurrencyId2,
							BaseSymbol = t.Currency2.Symbol,
							BaseName = t.Currency2.Name,
							BaseFee = t.Currency2.TradeFee,
							BaseMinTrade = t.Currency2.MinBaseTrade,
							Change = t.Change,
							LastTrade = t.LastTrade,
							Status = t.Status,
							StatusMessage = t.StatusMessage
						}).ToListNoLockAsync().ConfigureAwait(false);
					return summary;
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		//public async Task UpdatePriceCache(TradePriceUpdate priceUpdate)
		//{
		//	var tradePairs = await GetOrCacheTradePairs().ConfigureAwait(false);
		//	if (!tradePairs.IsNullOrEmpty())
		//	{
		//		var tradePair = tradePairs.FirstOrDefault(x => x.TradePairId == priceUpdate.TradePairId);
		//		if (tradePair == null)
		//			return;

		//		tradePair.Change = priceUpdate.Change;
		//		tradePair.LastTrade = priceUpdate.Last;

		//		var tradePairTicker = await GetTicker(tradePair.TradePairId).ConfigureAwait(false);
		//		if (tradePairTicker == null)
		//			return;

		//		tradePairTicker.High = priceUpdate.High;
		//		tradePairTicker.Low = priceUpdate.Low;
		//		tradePairTicker.Last = tradePair.LastTrade;
		//		tradePairTicker.Volume = priceUpdate.Volume;
		//		tradePairTicker.BaseVolume = priceUpdate.BaseVolume;
		//		tradePairTicker.Change = (decimal)priceUpdate.Change;
		//	}
		//}

		public async Task<TradePairTickerModel> GetTicker(int tradePairId)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.TradePairTicker(tradePairId), TimeSpan.FromSeconds(10), async () =>
			{
				var tradePair = await GetTradePair(tradePairId).ConfigureAwait(false);
				if (tradePair == null)
					return new TradePairTickerModel { Id = tradePairId };

				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var lastDate = DateTime.UtcNow.AddHours(-24);
					var history = await context.TradeHistory
					.AsNoTracking()
					.Where(x => x.TradePairId == tradePairId && x.Timestamp > lastDate)
					.OrderBy(x => x.Id)
					.Select(x => new
					{
						x.Rate,
						x.Amount
					}).ToListNoLockAsync().ConfigureAwait(false);
					if (history.IsNullOrEmpty())
						return new TradePairTickerModel
						{
							Id = tradePairId,
							Low = tradePair.LastTrade,
							High = tradePair.LastTrade,
							Last = tradePair.LastTrade,
						};

					return new TradePairTickerModel
					{
						Id = tradePairId,
						Last = tradePair.LastTrade,
						Change = (decimal)tradePair.Change,
						High = history.Max(x => x.Rate),
						Low = history.Min(x => x.Rate),
						Volume = history.Sum(x => x.Amount),
						BaseVolume = history.Sum(x => x.Amount * x.Rate)
					};
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<UpdateTradePairModel> AdminGetTradePair(int id)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				return await context.TradePair
				.AsNoTracking()
				.Select(x => new UpdateTradePairModel
				{
					Id = x.Id,
					Name = string.Concat(x.Currency1.Symbol, "_", x.Currency2.Symbol),
					Status = x.Status,
					StatusMessage = x.StatusMessage
				}).FirstOrDefaultNoLockAsync(x => x.Id == id).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> AdminGetTradePairs(DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.TradePair
				.AsNoTracking()
				.Select(x => new
				{
					Id = x.Id,
					Name = string.Concat(x.Currency1.Symbol, "_", x.Currency2.Symbol),
					Currency = x.Currency1.Name,
					BaseCurrency = x.Currency2.Name,
					Status = x.Status,
					StatusMessage = x.StatusMessage,
				});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}
	}
}
