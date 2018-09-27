using Cryptopia.Common.Arbitrage;
using Cryptopia.Common.Cache;
using Cryptopia.Infrastructure.Common.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;

namespace Cryptopia.Core.Arbitrage
{
	public class ArbitrageReader : IArbitrageReader
	{
		public ICacheService CacheService { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<List<ArbitrageDataModel>> GetData()
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ArbitrageData(), TimeSpan.FromMinutes(2), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					return await context.IntegrationMarketData
						.AsNoTracking()
						.Where(x => x.IntegrationExchange.IsEnabled && x.TradePair.Status != Enums.TradePairStatus.Closed)
						.Select(x => new ArbitrageDataModel
						{
							TradePairId = x.TradePairId,
							Currency = x.TradePair.Currency1.Name,
							Exchange = x.IntegrationExchange.Name,
							CurrencyId = x.TradePair.CurrencyId1,
							BaseCurrencyId = x.TradePair.CurrencyId2,
							Symbol = x.TradePair.Currency1.Symbol,
							BaseSymbol = x.TradePair.Currency2.Symbol,
							Ask = x.Ask,
							Bid = x.Bid,
							Last = x.Last,
							Volume = x.Volume,
							BaseVolume = x.BaseVolume,
							MarketUrl = x.MarketUrl
						})
						.GroupBy(x => x.TradePairId)
						.Where(x => x.Count() > 1)
						.SelectMany(x => x)
						.OrderBy(x => x.Currency)
						.ToListNoLockAsync().ConfigureAwait(false);
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}
	}
}
