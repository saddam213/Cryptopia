using Cryptopia.Common.DataContext;
using Cryptopia.Common.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Enums;
using Cryptopia.Base;
using Cryptopia.Common.Cache;
using System.Collections.Concurrent;
using Cryptopia.Common.Currency;
using Cryptopia.Common.TradePair;
using Cryptopia.Common.TradeNotification;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Infrastructure.Common.DataContext;

namespace Cryptopia.Core.Exchange
{
	public class ExchangeReader : IExchangeReader
	{
		public ICacheService CacheService { get; set; }
		public ICurrencyReader CurrencyReader { get; set; }
		public ITradePairReader TradePairReader { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetExchangeSummary(DataTablesModel param, string baseCurrency)
		{
			var results = await GetOrCacheBaseSummary(baseCurrency).ConfigureAwait(false);
			return results.GetDataTableResult(param, true);
		}

		public async Task<ExchangeSummary> GetExchangeSummary()
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ExchangeSummary(), TimeSpan.FromSeconds(20), async () =>
			{
				var tradeHistoryData = await GetOrCacheTradeHistory().ConfigureAwait(false);
				var tradePairs = await TradePairReader.GetTradePairs().ConfigureAwait(false);
				var currencies = await CurrencyReader.GetCurrencies().ConfigureAwait(false);
				var baseCurrencies = await CurrencyReader.GetBaseCurrencies().ConfigureAwait(false);
				var summaryData = new List<ExchangeSummaryModel>();

				foreach (var x in tradePairs)
				{
					var data = tradeHistoryData.Where(t => t.TradePairId == x.TradePairId);
					summaryData.Add(new ExchangeSummaryModel
					{
						TradePairId = x.TradePairId,
						Symbol = x.Symbol,
						BaseSymbol = x.BaseSymbol,
						TotalTrades = data.Count(),
						Low = (data.OrderBy(t => t.Rate).FirstOrDefault()?.Rate ?? x.LastTrade),
						High = (data.OrderByDescending(t => t.Rate).FirstOrDefault()?.Rate ?? x.LastTrade),
						Volume = data.Sum(t => t.Amount),
						TotalBase = data.Sum(t => t.Amount * t.Rate),
						Change = GetChangePercent(data.FirstOrDefault()?.Rate ?? x.LastTrade, x.LastTrade)
					});
				}

				var now = DateTime.UtcNow;
				var baseData = summaryData.GroupBy(x => x.BaseSymbol);
				var totalVolume = baseData.Select(x => new { Symbol = x.Key, Total = x.Sum(u => u.TotalBase) }).ToList();
				var topMarket = baseData.Select(x => x.MaxBy(u => u.TotalBase)).OrderBy(x => x.BaseSymbol).ToList();
				var featured = currencies.Any(x => x.FeaturedExpires > now)
					? currencies.Where(x => x.FeaturedExpires > now).ToList()
					: currencies.Where(x => x.CurrencyId == Constant.DOTCOIN_ID).ToList();

				var baseCurrencyData = new List<ExchangeSummaryBaseCurrency>();
				foreach (var currency in baseCurrencies)
				{
					var totalVol = totalVolume.FirstOrDefault(x => x.Symbol == currency.Symbol);
					baseCurrencyData.Add(new ExchangeSummaryBaseCurrency
					{
						CurrencyId = currency.CurrencyId,
						Name = currency.Name,
						Symbol = currency.Symbol,
						TotalVolume = totalVol != null
							? totalVol.Total
							: 0,
						Rank = currency.Rank
					});
				}

				return new ExchangeSummary
				{
					TopMarkets = topMarket,
					BaseCurrencies = baseCurrencyData,
					TotalMarkets = tradePairs.Count,
					TotalTrades = summaryData.Sum(x => x.TotalTrades),
					Featured = featured.Select(x => new FeaturedCurrency
					{
						Name = x.Name,
						Symbol = x.Symbol,
						Website = x.Website,
						Summary = x.Summary
					}).ToList()
				};

			}).ConfigureAwait(false);
			return cacheResult;
		}

		//public async Task UpdatePriceCache(TradePriceUpdate priceUpdate)
		//{
		//	var baseCurrency = priceUpdate.Market.Split('_')[1];
		//	var baseSummary = await GetExchangeSummary().ConfigureAwait(false);
		//	var summary = await GetOrCacheBaseSummary(baseCurrency).ConfigureAwait(false);
		//	var summaryItem = summary.FirstOrDefault(x => x.TradePairId == priceUpdate.TradePairId);
		//	var baseSummaryItem = baseSummary.TopMarkets.FirstOrDefault(x => x.TradePairId == priceUpdate.TradePairId);
		//	if (summaryItem != null)
		//	{
		//		summaryItem.Low = priceUpdate.Low.ToString("F8");
		//		summaryItem.High = priceUpdate.High.ToString("F8");
		//		summaryItem.Last = priceUpdate.Last.ToString("F8");
		//		summaryItem.Change = priceUpdate.Change.ToString("F2");
		//		summaryItem.Volume = priceUpdate.Volume.ToString("F8");
		//		summaryItem.BaseVolume = priceUpdate.BaseVolume.ToString("F8");
		//	}

		//	baseSummary.TotalTrades++;
		//	if (baseSummaryItem != null)
		//	{
		//		baseSummaryItem.TotalTrades++;
		//		baseSummaryItem.Change = priceUpdate.Change;
		//		baseSummaryItem.Volume = priceUpdate.Volume;
		//		baseSummaryItem.TotalBase = priceUpdate.BaseVolume;
		//		baseSummaryItem.High = priceUpdate.High;
		//		baseSummaryItem.Low = priceUpdate.Low;
		//	}
		//	await CacheService.UpdateAsync(CacheKey.ExchangeSummary(), baseSummary);
		//	await CacheService.UpdateAsync(CacheKey.ExchangeSummary(baseCurrency), summary);
		//}

		public async Task<StockChartDataModel> GetStockChart(int tradePairId, int dataRange, int dataGroup)
		{
			var cacheRange = GetCacheRange(dataRange);
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ExchangeStockChart(tradePairId, cacheRange, dataGroup), TimeSpan.FromMinutes(1), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					Entity.TradeHistory lastTrade;
					var minutes = dataGroup;//minuteGroups;
					var chartData = new ConcurrentBag<decimal[]>();
					var lastTime = GetChartRange(cacheRange);

					var tradePairData = await context.TradeHistory
							.AsNoTracking()
							.Where(th => th.TradePairId == tradePairId && th.Timestamp >= lastTime)
							.OrderBy(x => x.Id)
							//.Take(10000)
							.Select(th => new
							{
								Timestamp = th.Timestamp,
								Rate = th.Rate,
								Amount = th.Amount
							}).ToListNoLockAsync().ConfigureAwait(false);
					if (tradePairData.IsNullOrEmpty())
					{
						lastTrade = await context.TradeHistory
							.AsNoTracking()
							.Where(th => th.TradePairId == tradePairId)
							.OrderByDescending(x => x.Id)
							.FirstOrDefaultNoLockAsync().ConfigureAwait(false);

						if (lastTrade == null)
						{
							return new StockChartDataModel
							{
								Candle = new List<decimal[]> { new decimal[] { 0, 0, 0, 0, 0, 0 } },
								Volume = new List<VolumePoint> { new VolumePoint { x = 0, y = 0, basev = 0 } }
							};
						}

						var lastfinish = DateTime.UtcNow;
						var lasttotalhours = (int)(lastfinish - lastTime).TotalMinutes / minutes;
						for (int i = 0; i < lasttotalhours; i++)
						{
							chartData.Add(new[] { lastfinish.AddMinutes(-(minutes * i)).ToJavaTime(), lastTrade.Rate, lastTrade.Rate, lastTrade.Rate, lastTrade.Rate, 0, 0 });
						}
					}
					else
					{

						var start = tradePairData.Min(x => x.Timestamp);
						var finish = DateTime.UtcNow;
						var totalhours = (int)(finish - start).TotalMinutes / minutes;
						var interval = TimeSpan.FromMinutes(minutes);

						var dataTest = tradePairData.GroupBy(cr => new
						{
							cr.Timestamp.Year,
							Month = minutes >= 43200 ? (cr.Timestamp.Month / (minutes / 43200)) : cr.Timestamp.Month,
							Day = minutes >= 1440 ? (cr.Timestamp.Day / (minutes / 1440)) : cr.Timestamp.Day,
							Hour = minutes >= 60 ? (cr.Timestamp.Hour / (minutes / 60)) : cr.Timestamp.Hour,
							Minute = minutes < 60 ? (cr.Timestamp.Minute / minutes) * minutes : 0
						}).Select(g => new[]
						{
							g.Last().Timestamp.ToJavaTime(),
							g.First().Rate,
							g.Max(x => x.Rate),
							g.Min(x => x.Rate),
							g.Last().Rate,
							g.Sum(x => x.Amount),
							g.Sum(x => x.Amount * x.Rate)
						}).ToList();

						for (int i = 0; i < totalhours; i++)
						{
							var currentInterval = finish.AddMinutes(-(minutes * i));
							var rangeEnd = currentInterval.ToJavaTime();
							var rangeStart = currentInterval.Subtract(interval).ToJavaTime();
							var data = dataTest.FirstOrDefault(x => x[0] >= rangeStart && x[0] <= rangeEnd);
							if (data.IsNullOrEmpty())
							{
								var lastClose = dataTest.LastOrDefault(x => x[0] < rangeEnd)?[4] ?? 0m;
								chartData.Add(new[] { rangeEnd, lastClose, lastClose, lastClose, lastClose, 0, 0 });
							}
							else
							{
								data[0] = rangeEnd;
								chartData.Add(data);
							}
						}
					}

					var resultData = chartData.OrderBy(x => x[0]);
					var val = new StockChartDataModel
					{
						Candle = resultData.Select(x => new[] { x[0], x[1], x[2], x[3], x[4] }).ToList(),
						Volume = resultData.Select(x => new VolumePoint { x = x[0], y = x[5], basev = x[6] }).ToList()
					};

					return val;
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<DistributionChartDataModel> GetDistributionChart(int currencyId, ChartDistributionCount count)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ExchangeDistributionChart(currencyId, count), TimeSpan.FromMinutes(2), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var distributionData = await context.Balance
						.AsNoTracking()
						.Where(x => x.CurrencyId == currencyId && x.Total > 0 && !Constant.SYSTEM_ACCOUNTS.Contains(x.UserId))
						.OrderByDescending(x => x.Total)
						.Take((int)count)
						.Select(x => x.Total)
						.ToListNoLockAsync().ConfigureAwait(false);
					return new DistributionChartDataModel
					{
						Distribution = distributionData
					};
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<List<string[]>> GetTradeHistory(int tradePairId)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ExchangeTradeHistory(tradePairId), TimeSpan.FromMinutes(60), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var query = await context.TradeHistory
					.AsNoTracking()
					.Where(x => x.TradePairId == tradePairId).OrderByDescending(x => x.Id).Take(100).Select(x => new
					{
						x.Timestamp,
						x.Type,
						x.Rate,
						x.Amount
					}).ToListNoLockAsync().ConfigureAwait(false);
					return query.Select(x => new[]
					{
						x.Timestamp.ToString(),
						x.Type.ToString(),
						x.Rate.ToString("F8"),
						x.Amount.ToString("F8"),
						(x.Amount * x.Rate).ToString("F8")
					}).ToList();
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<OrderBookModel> GetOrderBook(string userId, int tradePairId)
		{
			return await GetOrCacheOrderData(tradePairId).ConfigureAwait(false);
		}

		public async Task<List<string[]>> GetUserOrders(string userId, int tradePairId)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ExchangeUserOpenOrders(userId, tradePairId), TimeSpan.FromMinutes(60), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var currentUser = new Guid(userId);
					var open = await context.Trade
							.AsNoTracking()
							.Where(x => x.UserId == currentUser && x.TradePairId == tradePairId && (x.Status == TradeStatus.Partial || x.Status == TradeStatus.Pending))
							.OrderByDescending(x => x.Id)
							.Select(x => new
							{
								Timestamp = x.Timestamp,
								Type = x.Type,
								Rate = x.Rate,
								Amount = x.Amount,
								Remaining = x.Remaining,
								TradeId = x.Id
							}).ToListNoLockAsync().ConfigureAwait(false);

					var orderData = open.Select(x => new[]
					{
						x.Timestamp.ToString(),
						x.Type.ToString(),
						x.Rate.ToString("F8"),
						x.Amount.ToString("F8"),
						x.Remaining.ToString("F8"),
						(x.Amount * x.Rate).ToString("F8"),
						x.TradeId.ToString()
					});

					return orderData.ToList();
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<List<string[]>> GetUserHistory(string userId, int tradePairId)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ExchangeUserTradeHistory(userId, tradePairId), TimeSpan.FromMinutes(60), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var currentUser = new Guid(userId);
					var history = await context.TradeHistory
						.AsNoTracking()
						.Where(x => (x.UserId == currentUser || x.ToUserId == currentUser) && x.TradePairId == tradePairId)
						.OrderByDescending(x => x.Id)
						.Take(100)
						.Select(x => new
						{
							Timestamp = x.Timestamp,
							Type = x.ToUserId == currentUser ? TradeHistoryType.Sell : TradeHistoryType.Buy,
							Rate = x.Rate,
							Amount = x.Amount,
						}).ToListNoLockAsync().ConfigureAwait(false);

					var historydata = history.Select(x => new[]
					{
					x.Timestamp.ToString(),
					x.Type.ToString(),
					x.Rate.ToString("F8"),
					x.Amount.ToString("F8"),
					(x.Amount * x.Rate).ToString("F8")
				});

					return historydata.ToList();
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<DataTablesResponse> GetUserOrders(DataTablesModel param, string userId)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ExchangeUserOpenOrderDataTable(userId), TimeSpan.FromMinutes(60), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var currentUser = new Guid(userId);
					var open = await context.Trade
						.AsNoTracking()
						.Where(x => x.UserId == currentUser && (x.Status == TradeStatus.Partial || x.Status == TradeStatus.Pending))
						.OrderByDescending(x => x.Id)
						.Select(x => new
						{
							Timestamp = x.Timestamp,
							Type = x.Type,
							Rate = x.Rate,
							Amount = x.Amount,
							Remaining = x.Remaining,
							TradeId = x.Id,
							TradePairId = x.TradePairId,
							Symbol1 = x.TradePair.Currency1.Symbol,
							Symbol2 = x.TradePair.Currency2.Symbol
						}).ToListNoLockAsync().ConfigureAwait(false);

					var orderData = open.Select(x => new
					{
						Market = string.Format("{0}/{1}", x.Symbol1, x.Symbol2),
						Type = x.Type.ToString(),
						Rate = x.Rate.ToString("F8"),
						Remaining = x.Remaining.ToString("F8"),
						TradeId = x.TradeId.ToString(),
						TradePairId = x.TradePairId.ToString()
					}).GetDataTableResult(param, true);

					return orderData;
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		private async Task<OrderBookModel> GetOrCacheOrderData(int tradePairId)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ExchangeOrderBook(tradePairId), TimeSpan.FromMinutes(60), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var orderbookData = await context.Trade
					.AsNoTracking()
					.Where(x => x.TradePairId == tradePairId && (x.Status == TradeStatus.Partial || x.Status == TradeStatus.Pending))
					.GroupBy(x => new { x.Type, x.TradePair.Currency1.Symbol, x.Rate })
					.Select(x => new OrderData
					{
						Symbol = x.Key.Symbol,
						Rate = x.Key.Rate,
						Amount = x.Sum(c => c.Remaining),
						Type = x.Key.Type
					}).ToListNoLockAsync().ConfigureAwait(false);

					var buyResults = new List<string[]>();
					foreach (var trade in orderbookData.Where(x => x.Type == TradeHistoryType.Buy).OrderByDescending(x => x.Rate))
					{
						buyResults.Add(new[]
						{
							"",
								trade.Rate.ToString("F8"),
								trade.Amount.ToString("F8"),
								(trade.Amount*trade.Rate).ToString("F8"),
								""
							});
					}

					var sellResults = new List<string[]>();
					foreach (var trade in orderbookData.Where(x => x.Type == TradeHistoryType.Sell).OrderBy(x => x.Rate))
					{
						sellResults.Add(new[]
						{
							"",
								trade.Rate.ToString("F8"),
								trade.Amount.ToString("F8"),
								(trade.Amount*trade.Rate).ToString("F8"),
								""
							});
					}

					return new OrderBookModel
					{
						SellData = sellResults,
						BuyData = buyResults
					};
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		private async Task<List<TradeHistoryData>> GetOrCacheTradeHistory()
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ExchangeTradeHistoryData(), TimeSpan.FromMinutes(1), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var lastTime = DateTime.UtcNow.AddHours(-24);
					var results = await context.TradeHistory
						.AsNoTracking()
						.Where(tp => tp.TradePair.Status != TradePairStatus.Closed && tp.Timestamp > lastTime)
						.Select(x => new TradeHistoryData
						{
							TradePairId = x.TradePairId,
							Amount = x.Amount,
							Rate = x.Rate,
							Timestamp = x.Timestamp
						}).ToListNoLockAsync();
					return results.OrderBy(x => x.Timestamp).ToList();
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		private async Task<List<TradeSummaryData>> GetOrCacheBaseSummary(string baseCurrency)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ExchangeSummary(baseCurrency), TimeSpan.FromMinutes(5), async () =>
			{
				var tradeHistory = await GetOrCacheTradeHistory().ConfigureAwait(false);
				var tradePairs = await TradePairReader.GetTradePairs().ConfigureAwait(false);
				var basepairs = tradePairs.Where(x => x.BaseSymbol == baseCurrency).ToList();
				var ids = basepairs.Select(x => x.TradePairId);
				var tradeHistoryData = tradeHistory.Where(tp => ids.Contains(tp.TradePairId)).ToList();

				var results = new List<TradeSummaryData>();
				foreach (var x in basepairs)
				{
					var data = tradeHistoryData.Where(t => t.TradePairId == x.TradePairId);
					results.Add(new TradeSummaryData
					{
						CurrencyId = x.CurrencyId,
						TradePairId = x.TradePairId,
						Name = x.Name,
						Symbol = x.Symbol,
						Change = GetChangePercent(data.FirstOrDefault()?.Rate ?? x.LastTrade, x.LastTrade).ToString("F2"),
						BaseVolume = data.Sum(t => t.Amount * t.Rate).ToString("F8"),
						Volume = data.Sum(t => t.Amount).ToString("F8"),
						Low = (data.OrderBy(t => t.Rate).FirstOrDefault()?.Rate ?? x.LastTrade).ToString("F8"),
						High = (data.OrderByDescending(t => t.Rate).FirstOrDefault()?.Rate ?? x.LastTrade).ToString("F8"),
						Last = x.LastTrade.ToString("F8"),
						BaseSymbol = x.BaseSymbol
					});
				}

				return results.ToList();
			}).ConfigureAwait(false);
			return cacheResult;
		}

		private static int GetCacheRange(int dataRange)
		{
			switch (dataRange)
			{
				case 0:
				case 1:
					return 0;
				case 2:
				case 3:
					return 1;
				case 4:
				case 5:
					return 2;
				case 6:
					return 3;
				case 7:
					return 4;
				default:
					break;
			}
			return 0;
		}

		private static DateTime GetChartRange(int dataRange)
		{
			switch (dataRange)
			{
				case 0:
					return DateTime.UtcNow.AddMonths(-1);
				case 1:
					return DateTime.UtcNow.AddMonths(-3);
				case 2:
					return DateTime.UtcNow.AddMonths(-6);
				case 3:
					return DateTime.UtcNow.AddYears(-1);
				case 4:
					return DateTime.UtcNow.AddYears(-6);
				default:
					break;
			}
			return DateTime.UtcNow.AddDays(-1);
		}

		private static double GetChangePercent(decimal lastTrade, decimal newTrade)
		{
			if (lastTrade > 0)
			{
				return Math.Round((double)((newTrade - lastTrade) / lastTrade * 100m), 2);
			}
			return 0.00;
		}




	}
}
