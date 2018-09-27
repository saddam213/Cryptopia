using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Cryptopia.Base.Logging;
using Cryptopia.Base;

namespace Cryptopia.IntegrationService.Exchanges
{
	public class CcexExchange : IExchange
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(CcexExchange));

		public int Id
		{
			get { return 7; }
		}

		public string Name
		{
			get { return "C-cex"; }
		}

		public string Url
		{
			get { return "https://c-cex.com/t/"; }
		}

		public async Task<List<IExchangeMarket>> GetMarketData(Dictionary<string, int> tradePairMap)
		{
			try
			{
				Log.Message(LogLevel.Info, "GetMarketData - Start");
				using (var client = new HttpClient())
				{
					var data = new List<IExchangeMarket>();
					var response = await client.GetStringAsync("https://c-cex.com/t/pairs.json");
					if (!string.IsNullOrEmpty(response))
					{
						var pairNames = JObject.Parse(response).SelectToken("pairs").ToObject<List<string>>();
						if (pairNames.IsNullOrEmpty())
						{
							throw new Exception("No tradepairs returned");
						}

						var ourPairs = tradePairMap.Keys.Select(x => x.Replace('_','-').ToLower());
						pairNames = pairNames.Where(x => ourPairs.Contains(x)).ToList();
						foreach (var pairName in pairNames)
						{
							try
							{
								var pairData = await client.GetStringAsync(string.Format("https://c-cex.com/t/{0}.json", pairName));
								if (!string.IsNullOrEmpty(pairData))
								{
									var pairInfo = JObject.Parse(pairData).SelectToken("ticker").ToObject<CcexMarket>();
									if (pairInfo != null)
									{
										pairInfo.market = pairName;
										data.Add(pairInfo);
									}
								}
							}
							catch (Exception ex)
							{
								Log.Exception("An exception occured fetching tradepair data, TradePair: {0}", ex, pairName);
							}
						}
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

		public class CcexMarket : IExchangeMarket
		{
			public string market { get; set; }
			public decimal lastprice { get; set; }
			public decimal sell { get; set; }
			public decimal buy { get; set; }
			public decimal vol { get; set; }
			public decimal vol_cur { get; set; }

			public string TradePair
			{
				get { return string.IsNullOrEmpty(market) ? string.Empty : market.Replace('-','_').ToUpper(); }
			}

			public decimal Bid
			{
				get { return buy; }
			}

			public decimal Ask
			{
				get { return sell; }
			}

			public decimal Last
			{
				get { return lastprice; }
			}

			public decimal Volume
			{
				get { return -1M; }
			}

			public decimal BaseVolume
			{
				get { return -1M; }
			}

			public string MarketUrl
			{
				get { return string.Format("https://c-cex.com/?p={0}", market); }
			}
		}
	}
}
