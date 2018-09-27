using Cryptopia.Base.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace Cryptopia.IntegrationService.Exchanges
{
	public class LiveCoinExchange : IExchange
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(LiveCoinExchange));

		public int Id
		{
			get { return 10; }
		}

		public string Name
		{
			get { return "LiveCoin"; }
		}

		public string Url
		{
			get { return "https://api.livecoin.net/exchange/ticker"; }
		}

		public async Task<List<IExchangeMarket>> GetMarketData(Dictionary<string, int> tradePairMap)
		{
			try
			{
				Log.Message(LogLevel.Info, "GetMarketData - Start");
				using (var client = new HttpClient())
				{
					var response = await client.GetStringAsync(Url);
					if (!string.IsNullOrEmpty(response))
					{
						var data = JArray.Parse(response).ToObject<List<LiveCoinMarket>>().ToList<IExchangeMarket>();
						Log.Message(LogLevel.Info, "GetMarketData - End");
						return data;
					}
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

		public class LiveCoinMarket : IExchangeMarket
		{
			public string symbol { get; set; }
			public decimal? last { get; set; }
			public decimal? high { get; set; }
			public decimal? low { get; set; }
			public decimal? volume { get; set; }
			public decimal? max_bid { get; set; }
			public decimal? min_ask { get; set; }
			public decimal? best_bid { get; set; }
			public decimal? best_ask { get; set; }

			public string TradePair
			{
				get { return symbol.Replace("/","_"); }
			}

			public decimal Bid
			{
				get { return max_bid ?? 0; }
			}

			public decimal Ask
			{
				get { return min_ask ?? 0; }
			}

			public decimal Last
			{
				get { return last ?? 0; }
			}

			public decimal Volume
			{
				get { return volume ?? 0; }
			}

			public decimal BaseVolume
			{
				get { return -1; }
			}

			public string MarketUrl
			{
				get { return string.Format("https://www.livecoin.net"); }
			}
		}
	}
}
