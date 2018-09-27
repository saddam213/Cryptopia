using Cryptopia.Common.Balance;
using Cryptopia.Common.Cache;
using Cryptopia.Common.External;
using Cryptopia.Common.TradePair;
using Cryptopia.Enums;
using System;
using System.Threading.Tasks;

namespace Cryptopia.Core.Balance
{
	public class BalanceEstimationService : IBalanceEstimationService
	{
		public ICacheService CacheService { get; set; }
		public ITradePairReader TradePairReader { get; set; }
		public IExternalApiService ExternalApiService { get; set; }

		public async Task<decimal> GetEstimatedValue(decimal amount, int currencyId, int baseCurrency)
		{
			var tradePair = await TradePairReader.GetTradePair(currencyId, baseCurrency).ConfigureAwait(false);
			if (tradePair != null)
			{
				return amount * tradePair.LastTrade;
			}
			return 0;
		}

		public async Task<decimal> GetEstimatedBTC(decimal amount, int currencyId)
		{
			if (amount == 0)
				return 0;

			if (currencyId == Constant.BITCOIN_ID)
				return amount;

			if (currencyId == Constant.USDT_ID)
			{
				var btc_usdt = await TradePairReader.GetTradePair(Constant.BITCOIN_ID, Constant.USDT_ID).ConfigureAwait(false);
				if (btc_usdt == null || btc_usdt.LastTrade == 0)
					return 0;

				return amount / btc_usdt.LastTrade;
			}

			if (currencyId == Constant.NZDT_ID)
			{
				var btc_nzdt = await TradePairReader.GetTradePair(Constant.BITCOIN_ID, Constant.NZDT_ID).ConfigureAwait(false);
				if (btc_nzdt == null || btc_nzdt.LastTrade == 0)
					return 0;

				return amount / btc_nzdt.LastTrade;
			}

			var tradePair = await TradePairReader.GetTradePair(currencyId, Constant.BITCOIN_ID).ConfigureAwait(false);
			if (tradePair == null)
			{
				var ltcTradePair = await TradePairReader.GetTradePair(currencyId, Constant.LITECOIN_ID).ConfigureAwait(false);
				var ltcbtcTradePair = await TradePairReader.GetTradePair(Constant.LITECOIN_ID, Constant.BITCOIN_ID).ConfigureAwait(false);
				if (ltcTradePair == null || ltcbtcTradePair == null)
					return 0;

				return (amount * ltcTradePair.LastTrade) * ltcbtcTradePair.LastTrade;
			}

			return amount * tradePair.LastTrade;
		}


		public async Task<decimal> GetEstimatedAmount(decimal btcAmount, int currencyId)
		{
			if (currencyId == Constant.BITCOIN_ID)
				return btcAmount;

			var tradePair = await TradePairReader.GetTradePair(currencyId, Constant.BITCOIN_ID).ConfigureAwait(false);
			if (tradePair == null)
			{
				var ltcTradePair = await TradePairReader.GetTradePair(currencyId, Constant.LITECOIN_ID).ConfigureAwait(false);
				var ltcbtcTradePair = await TradePairReader.GetTradePair(Constant.LITECOIN_ID, Constant.BITCOIN_ID).ConfigureAwait(false);
				if (ltcTradePair == null || ltcbtcTradePair == null)
					return 0;

				return (btcAmount / Math.Max(0.00000001m, ltcTradePair.LastTrade)) * Math.Max(0.00000001m, ltcbtcTradePair.LastTrade);
			}

			return btcAmount / Math.Max(0.00000001m, tradePair.LastTrade);
		}


		public async Task<decimal> GetEstimatedNZD(decimal amount, int currencyId)
		{
			if (amount == 0)
				return 0;

			if (currencyId == Constant.NZDT_ID)
				return amount;

			var estBtc = await GetEstimatedBTC(amount, currencyId);
			if (estBtc == 0)
				return 0;

			var nzdPrice = await ExternalApiService.ConvertDollarToBTC(1, ConvertDollarToBTCType.NZD);
			if (nzdPrice != 0)
				return estBtc / nzdPrice;

			var btc_nzdt = await TradePairReader.GetTradePair(Constant.BITCOIN_ID, Constant.NZDT_ID).ConfigureAwait(false);
			if (btc_nzdt == null || btc_nzdt.LastTrade == 0)
				return 0;

			return estBtc * btc_nzdt.LastTrade;
		}

		public async Task<decimal> GetNZDPerCoin(int currencyId)
		{
			var cacheResult = await CacheService.GetOrSetHybridValueAsync(CacheKey.NZDPerCoin(currencyId), TimeSpan.FromHours(2), async () =>
			{
				if (currencyId == Constant.NZDT_ID)
					return 1;

				var estBtc = await GetEstimatedBTC(1, currencyId);
				if (estBtc == 0)
					return 0;

				var nzdPrice = await ExternalApiService.ConvertDollarToBTC(1, ConvertDollarToBTCType.NZD);
				if (nzdPrice != 0)
					return estBtc / nzdPrice;

				var btc_nzdt = await TradePairReader.GetTradePair(Constant.BITCOIN_ID, Constant.NZDT_ID).ConfigureAwait(false);
				if (btc_nzdt == null || btc_nzdt.LastTrade == 0)
					return 0;

				return estBtc * btc_nzdt.LastTrade;
			});
			return cacheResult;
		}

	}
}
