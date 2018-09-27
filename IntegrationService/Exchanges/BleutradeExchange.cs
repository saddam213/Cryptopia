using Cryptopia.Base.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cryptopia.IntegrationService.Exchanges
{
	public class BleutradeExchange : IExchange
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(BleutradeExchange));

		public int Id
		{
			get { return 3; }
		}

		public string Name
		{
			get { return "Bleutrade"; }
		}

		public string Url
		{
			get { return "https://bleutrade.com/api/v2/public/getmarketsummaries"; }
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
						var data = JObject.Parse(response).SelectToken("result").ToObject<List<BleutradeMarket>>().ToList<IExchangeMarket>();
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

		public class BleutradeMarket : IExchangeMarket
		{
			public string MarketName { get; set; }
			public decimal High { get; set; }
			public decimal Low { get; set; }
			public decimal Last { get; set; }
			public decimal Volume { get; set; }
			public decimal BaseVolume { get; set; }
			public decimal Bid { get; set; }
			public decimal Ask { get; set; }

			public string TradePair
			{
				get { return MarketName.ToUpper(); }
			}

			public string MarketUrl
			{
				get { return string.Format("https://bleutrade.com/exchange/{0}", MarketName.Replace('_','/')); }
			}
		}
	}
}
