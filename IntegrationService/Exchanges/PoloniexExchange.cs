using Cryptopia.Base.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cryptopia.IntegrationService.Exchanges
{
	public class PoloniexExchange : IExchange
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(PoloniexExchange));

		public int Id
		{
			get { return 1; }
		}

		public string Name
		{
			get { return "Poloniex"; }
		}

		public string Url
		{
			get { return "https://poloniex.com/public?command=returnTicker"; }
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
						var data = JObject.Parse(response).Children().Select(x =>
						{
							var obj = x.Last.ToObject<PoloniexMarket>();
							obj.market = ((JProperty)x).Name;
							return obj;
						}).ToList<IExchangeMarket>();
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

		public class PoloniexMarket : IExchangeMarket
		{
			public string market { get; set; }
			public decimal last { get; set; }
			public decimal lowestAsk { get; set; }
			public decimal highestBid { get; set; }
			public double percentChange { get; set; }
			public decimal baseVolume { get; set; }
			public decimal quoteVolume { get; set; }

			public string TradePair
			{
				get { return string.Join("_", market.Split('_').Reverse()); }
			}

			public decimal Bid
			{
				get { return highestBid; }
			}

			public decimal Ask
			{
				get { return lowestAsk; }
			}

			public decimal Last
			{
				get { return last; }
			}

			public decimal Volume
			{
				get { return quoteVolume; }
			}

			public decimal BaseVolume
			{
				get { return baseVolume; }
			}

			public string MarketUrl
			{
				get { return string.Format("https://poloniex.com/exchange#{0}", market); }
			}
		}




	}
}
