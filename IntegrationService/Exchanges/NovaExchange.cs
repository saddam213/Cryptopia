using Cryptopia.Base.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace Cryptopia.IntegrationService.Exchanges
{
	public class NovaExchange : IExchange
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(NovaExchange));

		public int Id
		{
			get { return 9; }
		}

		public string Name
		{
			get { return "NovaExchange"; }
		}

		public string Url
		{
			get { return "https://novaexchange.com/remote/v2/markets"; }
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
						var data = JObject.Parse(response).SelectToken("markets").ToObject<List<NovaMarket>>().ToList<IExchangeMarket>();
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

		public class NovaMarket : IExchangeMarket
		{
			public string marketname { get; set; }
			public decimal? last_price { get; set; }
			public decimal? low24h { get; set; }
			public decimal? high24h { get; set; }
			public decimal? bid { get; set; }
			public decimal? ask { get; set; }
			public decimal? volume24h { get; set; }

			public string TradePair
			{
				get { return $"{marketname.Split('_')[1]}_{marketname.Split('_')[0]}"; }
			}

			public decimal Bid
			{
				get { return bid ?? 0; }
			}

			public decimal Ask
			{
				get { return ask ?? 0; }
			}

			public decimal Last
			{
				get { return last_price ?? 0; }
			}

			public decimal Volume
			{
				get { return -1; }
			}

			public decimal BaseVolume
			{
				get { return volume24h ?? 0; }
			}

			public string MarketUrl
			{
				get { return string.Format("https://novaexchange.com/market/{0}", marketname); }
			}
		}
	}
}
