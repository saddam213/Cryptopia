using Cryptopia.Base;
using Cryptopia.Common.Api;
using Cryptopia.Common.Cache;
using Cryptopia.Common.Currency;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.TradePair;
using Cryptopia.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;

namespace Cryptopia.Core.Api
{
	public class ApiPublicService : IApiPublicService
	{
		public ICacheService CacheService { get; set; }
		public ICurrencyReader CurrencyReader { get; set; }
		public ITradePairReader TradePairReader { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<ApiCurrencyResult> GetCurrencies()
		{
			var currencies = await CurrencyReader.GetCurrencies().ConfigureAwait(false);
			if (!currencies.Any())
				return new ApiCurrencyResult { Error = "No currencies found", Success = true, };

			var results = currencies.Select(x => new ApiCurrency
			{
				Algorithm = x.AlgoType.ToString(),
				DepositConfirmations = x.MinConfirmations,
				IsTipEnabled = x.TippingExpires > DateTime.UtcNow,
				MaxWithdraw = x.WithdrawMax,
				MinBaseTrade = x.MinBaseTrade,
				MinTip = x.TipMin,
				MinWithdraw = x.WithdrawMin,
				Name = x.Name,
				Status = x.Status.ToString(),
				StatusMessage = x.StatusMessage,
				Symbol = x.Symbol,
				WithdrawFee = x.WithdrawFee,
				Id = x.CurrencyId,
				ListingStatus = x.ListingStatus.ToString()
			});
			return new ApiCurrencyResult
			{
				Success = true,
				Data = new List<ApiCurrency>(results)
			};
		}

		public async Task<ApiTradePairResult> GetTradePairs()
		{
			var tradepairs = await TradePairReader.GetTradePairs().ConfigureAwait(false);
			if (!tradepairs.Any())
				return new ApiTradePairResult { Error = "No currencies found", Success = true, };

			var results = tradepairs.Select(x => new ApiTradePair
			{
				BaseCurrency = x.BaseName,
				BaseSymbol = x.BaseSymbol,
				Currency = x.Name,
				Label = x.Market.Replace('_', '/'),
				MaximumBaseTrade = 100000000m,
				MaximumPrice = 100000000m,
				MaximumTrade = 100000000m,
				MinimumBaseTrade = x.BaseMinTrade,
				MinimumPrice = 0.00000001m,
				MinimumTrade = 0.00000001m,
				Status = x.Status.ToString(),
				StatusMessage = x.StatusMessage,
				Symbol = x.Symbol,
				Id = x.TradePairId,
				TradeFee = x.BaseFee
			});

			return new ApiTradePairResult
			{
				Success = true,
				Data = new List<ApiTradePair>(results)
			};
		}

		public async Task<ApiMarketResult> GetMarket(string market, int hours)
		{
			var tradePair = await GetMarketTradePair(market).ConfigureAwait(false);
			if (tradePair == null)
				return new ApiMarketResult { Error = $"Market {market} not found", Success = true, };

			hours = Math.Min(hours, 168);
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ApiMarket(hours, tradePair.TradePairId), TimeSpan.FromSeconds(20), async () =>
			{
				var lastTime = DateTime.UtcNow.AddHours(-hours);
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var trades = await context.Trade
					.AsNoTracking()
					.Where(x => x.TradePairId == tradePair.TradePairId && (x.Status == Enums.TradeStatus.Partial || x.Status == Enums.TradeStatus.Pending))
					.Select(x => new
					{
						Type = x.Type,
						Rate = x.Rate,
						Amount = x.Amount
					}).ToListNoLockAsync().ConfigureAwait(false);

					var tradeHistory = await context.TradeHistory
					.AsNoTracking()
					.Where(x => x.TradePairId == tradePair.TradePairId && x.Timestamp > lastTime)
					.Select(x => new
					{
						Id = x.Id,
						Rate = x.Rate,
						Amount = x.Amount
					})
					.OrderBy(x => x.Id)
					.ToListNoLockAsync().ConfigureAwait(false);

					var ask = 0m;
					var bid = 0m;
					var buyVolume = 0m;
					var sellVolume = 0m;
					var buyBaseVolume = 0m;
					var sellBaseVolume = 0m;
					if (trades.Any())
					{
						var sellOrders = trades.Where(x => x.Type == Enums.TradeHistoryType.Sell).OrderBy(x => x.Rate).ToList();
						if (sellOrders.Any())
						{
							ask = sellOrders.First().Rate;
							sellVolume = Math.Round(sellOrders.Sum(x => x.Amount), 8);
							sellBaseVolume = Math.Round(sellOrders.Sum(x => x.Amount * x.Rate), 8);
						}
						var buyOrders = trades.Where(x => x.Type == Enums.TradeHistoryType.Buy).OrderByDescending(x => x.Rate).ToList();
						if (buyOrders.Any())
						{
							bid = buyOrders.First().Rate;
							buyVolume = Math.Round(buyOrders.Sum(x => x.Amount), 8);
							buyBaseVolume = Math.Round(buyOrders.Sum(x => x.Amount * x.Rate), 8);
						}
					}

					var change = 0m;
					var volume = 0m;
					var baseVolume = 0m;
					var open = tradePair.LastTrade;
					var close = tradePair.LastTrade;
					var high = tradePair.LastTrade;
					var low = tradePair.LastTrade;
					var last = tradePair.LastTrade;
					if (tradeHistory.Any())
					{
						var firstTrade = tradeHistory.FirstOrDefault();
						var lastTrade = tradeHistory.LastOrDefault();

						high = tradeHistory.Max(x => x.Rate);
						low = tradeHistory.Min(x => x.Rate);
						open = firstTrade.Rate;
						close = lastTrade.Rate;
						last = lastTrade.Rate;
						change = open.GetPercentChanged(close);
						volume = tradeHistory.Sum(x => x.Amount);
						baseVolume = Math.Round(tradeHistory.Sum(x => x.Amount * x.Rate), 8);
					}


					return new ApiMarketResult
					{
						Success = true,
						Data = new ApiMarket
						{
							Label = tradePair.Market.Replace("_", "/"),
							TradePairId = tradePair.TradePairId,
							Change = open.GetPercentChanged(close),
							AskPrice = ask,
							BidPrice = bid,
							BuyVolume = buyVolume,
							SellVolume = sellVolume,
							LastPrice = last,
							High = high,
							Low = low,
							Volume = volume,

							Open = open,
							Close = close,
							BaseVolume = baseVolume,
							BuyBaseVolume = buyBaseVolume,
							SellBaseVolume = sellBaseVolume
						}
					};
				}

			});
			return cacheResult;
		}

		public async Task<ApiMarketsResult> GetMarkets(string baseMarket, int hours)
		{
			hours = Math.Min(hours, 168);
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ApiMarkets(baseMarket, hours), TimeSpan.FromSeconds(20), async () =>
			{
				var lastTime = DateTime.UtcNow.AddHours(-hours);
				var tradePairs = await TradePairReader.GetTradePairs().ConfigureAwait(false);
				if (!tradePairs.Any())
					return new ApiMarketsResult { Error = $"Markets not found", Success = true, };

				var markets = string.IsNullOrEmpty(baseMarket) ? tradePairs : tradePairs.Where(x => x.BaseSymbol == baseMarket).ToList();
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var allTrades = await context.Trade
					.AsNoTracking()
					.Where(x => (x.Status == Enums.TradeStatus.Partial || x.Status == Enums.TradeStatus.Pending))
					.Select(x => new
					{
						TradePairId = x.TradePairId,
						Type = x.Type,
						Rate = x.Rate,
						Amount = x.Amount
					}).ToListNoLockAsync().ConfigureAwait(false);

					var allTradeHistory = await context.TradeHistory
					.AsNoTracking()
					.Where(x => x.Timestamp > lastTime)
					.Select(x => new
					{
						Id = x.Id,
						TradePairId = x.TradePairId,
						Rate = x.Rate,
						Amount = x.Amount
					})
					.OrderBy(x => x.Id)
					.ToListNoLockAsync().ConfigureAwait(false);

					var results = new ConcurrentBag<ApiMarket>();
					foreach (var tradePair in markets)
					{
						var trades = allTrades
							.Where(x => x.TradePairId == tradePair.TradePairId)
							.Select(x => new
							{
								Type = x.Type,
								Rate = x.Rate,
								Amount = x.Amount
							}).ToList();

						var tradeHistory = allTradeHistory
							.Where(x => x.TradePairId == tradePair.TradePairId)
							.Select(x => new
							{
								Id = x.Id,
								Rate = x.Rate,
								Amount = x.Amount
							})
							.OrderBy(x => x.Id)
							.ToList();

						var ask = 0m;
						var bid = 0m;
						var buyVolume = 0m;
						var sellVolume = 0m;
						var buyBaseVolume = 0m;
						var sellBaseVolume = 0m;
						if (trades.Any())
						{
							var sellOrders = trades.Where(x => x.Type == Enums.TradeHistoryType.Sell).OrderBy(x => x.Rate).ToList();
							if (sellOrders.Any())
							{
								ask = sellOrders.First().Rate;
								sellVolume = Math.Round(sellOrders.Sum(x => x.Amount), 8);
								sellBaseVolume = Math.Round(sellOrders.Sum(x => x.Amount * x.Rate), 8);
							}
							var buyOrders = trades.Where(x => x.Type == Enums.TradeHistoryType.Buy).OrderByDescending(x => x.Rate).ToList();
							if (buyOrders.Any())
							{
								bid = buyOrders.First().Rate;
								buyVolume = Math.Round(buyOrders.Sum(x => x.Amount), 8);
								buyBaseVolume = Math.Round(buyOrders.Sum(x => x.Amount * x.Rate), 8);
							}
						}

						var change = 0m;
						var volume = 0m;
						var baseVolume = 0m;
						var open = tradePair.LastTrade;
						var close = tradePair.LastTrade;
						var high = tradePair.LastTrade;
						var low = tradePair.LastTrade;
						var last = tradePair.LastTrade;
						if (tradeHistory.Any())
						{
							var firstTrade = tradeHistory.FirstOrDefault();
							var lastTrade = tradeHistory.LastOrDefault();

							high = tradeHistory.Max(x => x.Rate);
							low = tradeHistory.Min(x => x.Rate);
							open = firstTrade.Rate;
							close = lastTrade.Rate;
							last = lastTrade.Rate;
							change = open.GetPercentChanged(close);
							volume = tradeHistory.Sum(x => x.Amount);
							baseVolume = Math.Round(tradeHistory.Sum(x => x.Amount * x.Rate), 8);
						}

						results.Add(new ApiMarket
						{
							Label = tradePair.Market.Replace("_", "/"),
							TradePairId = tradePair.TradePairId,
							Change = open.GetPercentChanged(close),
							AskPrice = ask,
							BidPrice = bid,
							BuyVolume = buyVolume,
							SellVolume = sellVolume,
							LastPrice = last,
							High = high,
							Low = low,
							Volume = volume,

							Open = open,
							Close = close,
							BaseVolume = baseVolume,
							BuyBaseVolume = buyBaseVolume,
							SellBaseVolume = sellBaseVolume
						});
					}

					return new ApiMarketsResult
					{
						Success = true,
						Data = results.OrderBy(x => x.Label).ToList()
					};
				}
			});
			return cacheResult;
		}

		public async Task<ApiMarketHistoryResult> GetMarketHistory(string market, int hours)
		{
			var tradePair = await GetMarketTradePair(market).ConfigureAwait(false);
			if (tradePair == null)
				return new ApiMarketHistoryResult { Error = $"Market {market} not found", Success = true, };

			hours = Math.Min(hours, 168);
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ApiMarketHistory(tradePair.TradePairId, hours), TimeSpan.FromSeconds(20), async () =>
			{
				var lastTime = DateTime.UtcNow.AddHours(-hours);
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var marketData = await context.TradeHistory
					.AsNoTracking()
					.Where(x => x.TradePairId == tradePair.TradePairId && x.Timestamp > lastTime)
					.Select(x => new
					{
						TradePairId = x.TradePairId,
						Type = x.Type,
						Rate = x.Rate,
						Amount = x.Amount,
						Timestamp = x.Timestamp
					})
					.OrderByDescending(x => x.Timestamp)
					.Take(1000)
					.ToListNoLockAsync().ConfigureAwait(false);

					var results = marketData.Select(x => new ApiMarketHistory
					{
						Label = tradePair.Market.Replace("_", "/"),
						Amount = x.Amount,
						Price = x.Rate,
						Total = Math.Round(x.Amount * x.Rate, 8),
						Timestamp = (int)x.Timestamp.ToUnixTime(),
						TradePairId = tradePair.TradePairId,
						Type = x.Type.ToString()
					});

					return new ApiMarketHistoryResult
					{
						Success = true,
						Data = results.ToList()
					};
				}
			});
			return cacheResult;
		}

		public async Task<ApiMarketOrdersResult> GetMarketOrders(string market, int orderCount)
		{
			var tradePair = await GetMarketTradePair(market).ConfigureAwait(false);
			if (tradePair == null)
				return new ApiMarketOrdersResult { Error = $"Market {market} not found", Success = true, };

			var cacheResult = await CacheService.GetOrSetMemoryAsync(CacheKey.ApiMarketOrdersByTradePair(tradePair.TradePairId, orderCount), TimeSpan.FromSeconds(2), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var marketData = await context.Trade
					.AsNoTracking()
					.Where(x => x.TradePairId == tradePair.TradePairId && (x.Status == Enums.TradeStatus.Partial || x.Status == Enums.TradeStatus.Pending))
					.Select(x => new
					{
						TradePairId = x.TradePairId,
						Type = x.Type,
						Rate = x.Rate,
						Remaining = x.Remaining
					}).ToListNoLockAsync().ConfigureAwait(false);

					var marketBuyOrders = marketData
							.Where(x => x.Type == Enums.TradeHistoryType.Buy)
							.GroupBy(x => x.Rate)
							.OrderByDescending(x => x.Key)
							.Select(x => new ApiMarketOrder
							{
								TradePairId = tradePair.TradePairId,
								Label = tradePair.Market.Replace("_", "/"),
								Price = x.Key,
								Volume = Math.Round(x.Sum(b => b.Remaining), 8),
								Total = Math.Round(x.Sum(b => b.Remaining) * x.Key, 8)
							})
							.Take(orderCount)
							.ToList();

					var marketSellOrders = marketData
						.Where(x => x.Type == Enums.TradeHistoryType.Sell)
						.GroupBy(x => x.Rate)
						.OrderBy(x => x.Key)
						.Select(x => new ApiMarketOrder
						{
							TradePairId = tradePair.TradePairId,
							Label = tradePair.Market.Replace("_", "/"),
							Price = x.Key,
							Volume = Math.Round(x.Sum(b => b.Remaining), 8),
							Total = Math.Round(x.Sum(b => b.Remaining) * x.Key, 8)
						})
						.Take(orderCount)
						.ToList();

					return new ApiMarketOrdersResult
					{
						Success = true,
						Data = new ApiMarketOrderData
						{
							Buy = marketBuyOrders,
							Sell = marketSellOrders
						}
					};
				}
			});
			return cacheResult;
		}

		public async Task<ApiMarketOrderGroupResult> GetMarketOrderGroup(string tradePairs, int orderCount)
		{
			var cacheResult = await CacheService.GetOrSetMemoryAsync(CacheKey.ApiMarketOrderGroup(tradePairs, orderCount), TimeSpan.FromSeconds(2), async () =>
			{
				var tradePairIds = await GetMarketTradePairs(tradePairs).ConfigureAwait(false);
				if (!tradePairIds.Any())
					return new ApiMarketOrderGroupResult { Success = true, Message = "No markets found." };

				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var allMarketData = await context.Trade
					.AsNoTracking()
					.Where(x => (x.Status == Enums.TradeStatus.Partial || x.Status == Enums.TradeStatus.Pending) && tradePairIds.Contains(x.TradePairId))
					.Select(x => new
					{
						TradePairId = x.TradePairId,
						Currency1 = x.TradePair.Currency1.Symbol,
						Currency2 = x.TradePair.Currency2.Symbol,
						Type = x.Type,
						Rate = x.Rate,
						Remaining = x.Remaining
					}).ToListNoLockAsync().ConfigureAwait(false);

					var result = new List<ApiMarketOrderGroupData>();
					var marketsData = allMarketData.GroupBy(x => new { x.TradePairId, Market = string.Concat(x.Currency1, "_", x.Currency2) }).ToList();
					foreach (var marketData in marketsData)
					{
						var marketBuyOrders = marketData
							.Where(x => x.Type == Enums.TradeHistoryType.Buy)
							.GroupBy(x => x.Rate)
							.OrderByDescending(x => x.Key)
							.Select(x => new ApiMarketOrder
							{
								TradePairId = marketData.Key.TradePairId,
								Label = marketData.Key.Market,
								Price = x.Key,
								Volume = Math.Round(x.Sum(b => b.Remaining), 8),
								Total = Math.Round(x.Sum(b => b.Remaining) * x.Key, 8)
							})
							.Take(orderCount)
							.ToList();

						var marketSellOrders = marketData
							.Where(x => x.Type == Enums.TradeHistoryType.Sell)
							.GroupBy(x => x.Rate)
							.OrderBy(x => x.Key)
							.Select(x => new ApiMarketOrder
							{
								TradePairId = marketData.Key.TradePairId,
								Label = marketData.Key.Market,
								Price = x.Key,
								Volume = Math.Round(x.Sum(b => b.Remaining), 8),
								Total = Math.Round(x.Sum(b => b.Remaining) * x.Key, 8)
							})
							.Take(orderCount)
							.ToList();

						result.Add(new ApiMarketOrderGroupData
						{
							TradePairId = marketData.Key.TradePairId,
							Market = marketData.Key.Market,
							Buy = marketBuyOrders,
							Sell = marketSellOrders
						});
					}

					return new ApiMarketOrderGroupResult
					{
						Success = true,
						Data = result.OrderBy(x => x.Market).ToList()
					};
				}
			});
			return cacheResult;
		}

		private async Task<List<int>> GetMarketTradePairs(string marketList)
		{
			var result = new List<int>();
			if (string.IsNullOrEmpty(marketList))
				return result;

			var tradePairs = await TradePairReader.GetTradePairs().ConfigureAwait(false);
			if (marketList.Equals("All", StringComparison.OrdinalIgnoreCase))
				return tradePairs.Select(x => x.TradePairId).ToList();

			foreach (var market in marketList.Split('-'))
			{
				var parseId = 0;
				var match = int.TryParse(market, out parseId)
					? tradePairs.FirstOrDefault(x => x.TradePairId == parseId)
					: tradePairs.FirstOrDefault(x => x.Market == market.ToUpper());
				if (match != null)
				{
					result.Add(match.TradePairId);
				}
			}
			return result;
		}

		private async Task<TradePairModel> GetMarketTradePair(string market)
		{
			int marketId = 0;
			return int.TryParse(market, out marketId)
				? await TradePairReader.GetTradePair(marketId).ConfigureAwait(false)
				: await TradePairReader.GetTradePair(market).ConfigureAwait(false);
		}
	}
}
