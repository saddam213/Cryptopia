using Cryptopia.Base.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cryptopia.IntegrationService.Exchanges
{
	public class CryptopiaExchange : IExchange
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(CryptopiaExchange));

		public int Id
		{
			get { return 5; }
		}

		public string Name
		{
			get { return "Cryptopia"; }
		}

		public string Url
		{
			get { return "https://www.cryptopia.co.nz/api/GetMarkets"; }
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
						
						var data = JObject.Parse(response).SelectToken("Data").ToObject<List<CryptopiaMarket>>().ToList<IExchangeMarket>();
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

		public class CryptopiaMarket : IExchangeMarket
		{
			public string Label { get; set; }
			public decimal High { get; set; }
			public decimal Low { get; set; }
			public decimal Volume { get; set; }
			public decimal LastPrice { get; set; }
			public decimal AskPrice { get; set; }
			public decimal BidPrice { get; set; }
			public decimal BaseVolume { get; set; }

			public decimal Last
			{
				get { return LastPrice; }
			}
		
			public decimal Bid
			{
				get { return BidPrice; }
			}

			public decimal Ask
			{
				get { return AskPrice; }
			}

			public string TradePair
			{
				get { return Label.Replace('/', '_').ToUpper(); }
			}

			public string MarketUrl
			{
				get { return string.Format("https://www.cryptopia.co.nz/Exchange?market={0}", TradePair); }
			}
		}
	}
}
