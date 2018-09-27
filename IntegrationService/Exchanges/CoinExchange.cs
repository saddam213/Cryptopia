using Cryptopia.Base;
using Cryptopia.Base.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace Cryptopia.IntegrationService.Exchanges
{
	public class CoinExchange : IExchange
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(CoinExchange));

		public int Id
		{
			get { return 11; }
		}

		public string Name
		{
			get { return "CoinExchange"; }
		}

		public string Url
		{
			get { return "https://www.coinexchange.io/api/v1/getmarketsummaries"; }
		}

		public async Task<List<IExchangeMarket>> GetMarketData(Dictionary<string, int> tradePairMap)
		{
			try
			{
				Log.Message(LogLevel.Info, "GetMarketData - Start");
				using (var client = new HttpClient())
				{
					var marketResponse = await client.GetStringAsync("https://www.coinexchange.io/api/v1/getmarkets");
					if (string.IsNullOrEmpty(marketResponse))
						return new List<IExchangeMarket>();

					var markets = JObject.Parse(marketResponse).SelectToken("result").ToObject<List<CoinExchangeTradepair>>();
					if (markets.IsNullOrEmpty())
						return new List<IExchangeMarket>();

					var marketDataResponse = await client.GetStringAsync(Url);
					if (string.IsNullOrEmpty(marketDataResponse))
						return new List<IExchangeMarket>();


					var marketData = JObject.Parse(marketDataResponse).SelectToken("result").ToObject<List<CoinExchangeMarket>>();
					if (marketData.IsNullOrEmpty())
						return new List<IExchangeMarket>();

					foreach (var market in marketData)
					{
						market.SetTradePair(markets.FirstOrDefault(x => x.MarketID == market.MarketID));
					}

					Log.Message(LogLevel.Info, "GetMarketData - End");
					return marketData.ToList<IExchangeMarket>();

				}
			}
			catch (TaskCanceledException tex)
			{
				Log.Exception("Timeout", tex);
			}
			catch (Exception ex)
			{
				Log.Exception("An exception occured fetching API data", ex);
			}
			return new List<IExchangeMarket>();
		}

		public class CoinExchangeTradepair
		{
			public int MarketID { get; set; }
			public string MarketAssetCode { get; set; }
			public string BaseCurrencyCode { get; set; }

			public string TradePair
			{
				get { return string.Format("{0}_{1}", MarketAssetCode, BaseCurrencyCode); }
			}
		}

		public class CoinExchangeMarket : IExchangeMarket
		{
			public int MarketID { get; set; }
			public decimal? LastPrice { get; set; }
			public decimal? Change { get; set; }
			public decimal? HighPrice { get; set; }
			public decimal? LowPrice { get; set; }
			public decimal Volume { get; set; }
			public decimal? BTCVolume { get; set; }
			public decimal? TradeCount { get; set; }
			public decimal? BidPrice { get; set; }
			public decimal? AskPrice { get; set; }
			public decimal? BuyOrderCount { get; set; }
			public decimal? SellOrderCount { get; set; }

			public string TradePair { get; set; }

			public decimal Bid
			{
				get { return BidPrice ?? 0; }
			}

			public decimal Ask
			{
				get { return AskPrice ?? 0; }
			}

			public decimal Last
			{
				get { return LastPrice ?? 0; }
			}
		
			public decimal BaseVolume
			{
				get { return BTCVolume ?? 0; }
			}

			public string MarketUrl { get; set; }

			public void SetTradePair(CoinExchangeTradepair tradePair)
			{
				if (tradePair == null)
					return;

				TradePair = tradePair.TradePair;
				MarketUrl = string.Format("https://www.coinexchange.io/market/{0}/{1}", tradePair.MarketAssetCode, tradePair.BaseCurrencyCode);
			}
		}
	}
}
