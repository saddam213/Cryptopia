using Cryptopia.Base;
using Cryptopia.Base.Extensions;
using Cryptopia.Common.Cache;
using Cryptopia.Common.Currency;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.Mineshaft;
using Cryptopia.Common.Pool;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Core.Mineshaft
{
	public class MineshaftReader : IMineshaftReader
	{
		public ICacheService CacheService { get; set; }
		public IPoolReader PoolReader { get; set; }
		public ICurrencyReader CurrencyReader { get; set; }
		public IPoolDataContextFactory PoolDataContextFactory { get; set; }

		public async Task<MineshaftSummary> GetMineshaftSummary()
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.MineshaftSummary(), TimeSpan.FromSeconds(10), async () =>
			{
				var poolData = new List<MineshaftSummaryModel>();
				using (var context = PoolDataContextFactory.CreateContext())
				{
					poolData = await context.Pool
						.AsNoTracking()
						.Where(x => x.IsEnabled)
						.Select(x => new MineshaftSummaryModel
						{
							AlgoType = x.AlgoType,
							PoolId = x.Id,
							Symbol = x.Symbol,
							Name = x.Name,
							Hashrate = (double?)x.Statistics.Hashrate ?? 0,
							Luck = (double?)x.Blocks.OrderByDescending(b => b.Height).Take(25).Average(b => b.Luck) ?? 0,
							BlocksFound = x.Blocks.Count,
							Miners = (int?)x.UserStatistics.Count(b => b.Hashrate > 0) ?? 0
						}).ToListNoLockAsync().ConfigureAwait(false);
				}

				var algoPoolData = poolData.GroupBy(x => x.AlgoType)
						.Select(x => new
						{
							AlgoType = x.Key,
							TopPool = x.OrderByDescending(b => b.Hashrate).FirstOrDefault(),
							TotalHashrate = (double?)x.Sum(b => b.Hashrate) ?? 0
						});

				var now = DateTime.UtcNow;
				var pools = await PoolReader.GetPools().ConfigureAwait(false);
				var currencies = await CurrencyReader.GetCurrencies().ConfigureAwait(false);
				var featuredPools = pools.Any(x => x.FeaturedExpires > now)
					? pools.Where(x => x.FeaturedExpires > now).Select(x => new FeaturedPool(currencies.FirstOrDefault(c => c.CurrencyId == x.CurrencyId)))
					: pools.Where(x => x.CurrencyId == 2).Select(x => new FeaturedPool(currencies.FirstOrDefault(c => c.CurrencyId == x.CurrencyId)));

				var totalHashrate = pools.Sum(x => x.Hashrate);
				var topPools = poolData.OrderByDescending(x => x.Miners).Take(5).ToList();
				var algoInfo = algoPoolData.Select(x => new AlgoTypeInfo
				{
					Name = x.AlgoType.ToString(),
					AlgoType = x.AlgoType,
					TotalHashrate = x.TotalHashrate,
					TopPoolSymbol = x.TopPool.Symbol
				}).ToList();

				algoInfo.Insert(0, new AlgoTypeInfo
				{
					Name = "All Pools",
					AlgoType = null,
					TopPoolSymbol = topPools.FirstOrDefault()?.Symbol,
					TotalHashrate = totalHashrate
				});

				var model = new MineshaftSummary
				{
					TotalPools = pools.Count,
					TotalHashrate = totalHashrate,
					AlgoTypes = algoInfo,
					TopPools = topPools,
					Featured = featuredPools.ToList()
				};
				return model;

			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<DataTablesResponse> GetMineshaftSummary(DataTablesModel model, AlgoType? algoType)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.MineshaftSummary(algoType), TimeSpan.FromSeconds(10), async () =>
			{
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var query = context.Pool
						.AsNoTracking()
						.Where(x => x.IsEnabled && (!algoType.HasValue || x.AlgoType == algoType))
						.OrderBy(x => x.Symbol)
						.Select(pool => new
						{
							Id = pool.Id,
							Name = pool.Name,
							Symbol = pool.Symbol,
							AlgoType = pool.AlgoType,
							Miners = (double?)pool.UserStatistics.Count(x => x.Hashrate > 0) ?? 0,
							Difficulty = (double?)pool.Statistics.NetworkDifficulty ?? 0,
							Hashrate = (double?)pool.Statistics.Hashrate ?? 0,
							NetworkHashrate = (double?)pool.Statistics.NetworkHashrate ?? 0,
							Profitability = (double?)pool.Statistics.Profitability ?? 0,
							Status = pool.Status,
							SortHashrate = (double?)pool.Statistics.Hashrate ?? 0,
							SortNetworkHashrate = (double?)pool.Statistics.NetworkHashrate ?? 0
						});

					return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<MineshaftInfoModel> GetMineshaftInfo(int poolId)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.MineshaftInfo(poolId), TimeSpan.FromSeconds(10), async () =>
			{
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var query = from pool in context.Pool
											join connection in context.Connection on pool.AlgoType equals connection.AlgoType
											where pool.Id == poolId && pool.IsEnabled
											select new MineshaftInfoModel
											{
												Id = pool.Id,
												Name = pool.Name,
												Symbol = pool.Symbol,
												AlgoType = pool.AlgoType,
												Hashrate = pool.Statistics.Hashrate,
												NetworkHashrate = pool.Statistics.NetworkHashrate,
												NetworkDifficulty = pool.Statistics.NetworkDifficulty,
												InvalidShares = pool.Statistics.InvalidShares,
												CurrentBlock = pool.Statistics.CurrentBlock,
												LastPoolBlock = pool.Statistics.LastPoolBlock,
												LastBlockTime = pool.Statistics.LastBlockTime,
												ValidShares = pool.Statistics.ValidShares,
												BlockProgress = pool.Statistics.BlockProgress,
												EstimatedShares = pool.Statistics.EstimatedShares,
												EstimatedTime = pool.Statistics.EstimatedTime,
												Status = pool.Status,
												StatusMessage = pool.StatusMessage,
												StratumHost = connection.Host,
												StratumPort = connection.Port,
												BlocksFound = pool.Blocks.Count,
												UserCount = pool.UserStatistics.Count(x => x.Hashrate > 0),
												Luck = (double?)pool.Blocks.Average(x => x.Luck) ?? 0,
												Expires = pool.Expires,
												Profitability = pool.Statistics.Profitability
											};
					return await query.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<MineshaftUserInfoModel> GetMineshaftUserInfo(string userId, int poolId)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.MineshaftUserInfo(poolId, userId), TimeSpan.FromSeconds(10), async () =>
			{
				var currentUser = new Guid(userId);
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var result = await context.UserStatistics
											.AsNoTracking()
											.Where(p => p.PoolId == poolId && p.UserId == currentUser)
											.Select(p => new
											{
												ValidShares = p.ValidShares,
												InvalidShares = p.InvalidShares,
												Hashrate = p.Hashrate,
												WorkerCount = p.User.Workers.Count(x => x.IsEnabled && x.AlgoType == p.Pool.AlgoType),
												ActiveWorkerCount = p.User.Workers.Count(x => x.IsEnabled && x.AlgoType == p.Pool.AlgoType && x.IsActive && x.TargetPool == p.Pool.Symbol),
												Payments = p.User.Payouts
														.Where(x => x.PoolId == poolId && x.Block.Status != PoolBlockStatus.Orphan && (x.Status == PoolPayoutStatus.Unconfirmed || x.Status == PoolPayoutStatus.Pending || x.Status == PoolPayoutStatus.Confirmed))
														.Select(x => new
														{
															Status = x.Status,
															Amount = x.Amount
														}).ToList(),
											}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);

					return new MineshaftUserInfoModel
					{
						Hashrate = result.Hashrate,
						WorkerCount = result.WorkerCount,
						ValidShares = result.ValidShares,
						InvalidShares = result.InvalidShares,
						Confirmed = result.Payments.Where(x => x.Status == PoolPayoutStatus.Confirmed).Sum(x => x.Amount),
						Unconfirmed = result.Payments.Where(x => x.Status == PoolPayoutStatus.Unconfirmed || x.Status == PoolPayoutStatus.Pending).Sum(x => x.Amount),
						ActiveWorkerCount = result.ActiveWorkerCount
					};
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		private static string _query =
			@"SELECT
					MIN([Timestamp]) AS [Timestamp],
					MAX([Timestamp]) AS [LastTime],
					SUM(Difficulty) AS Shares
				FROM Shares_{0}
				WHERE [Timestamp] > @p0
				GROUP BY
				DATEPART(YEAR, [Timestamp]),
				DATEPART(MONTH, [Timestamp]),
				DATEPART(DAY, [Timestamp]),
				DATEPART(HOUR, [Timestamp]),
				(DATEPART(MINUTE,[Timestamp]) / 30)
				ORDER BY [Timestamp]";

		private static string _userQuery =
			@"SELECT
					MIN([Timestamp]) AS [Timestamp],
					MAX([Timestamp]) AS [LastTime],
					SUM(Difficulty) AS Shares
				FROM Shares_{0}
				WHERE UserId = @p0 AND [Timestamp] > @p1
				GROUP BY
				DATEPART(YEAR, [Timestamp]),
				DATEPART(MONTH, [Timestamp]),
				DATEPART(DAY, [Timestamp]),
				DATEPART(HOUR, [Timestamp]),
				(DATEPART(MINUTE,[Timestamp]) / 30)
				ORDER BY [Timestamp]";

		public async Task<HashrateChartModel> GetHashrateChart(int poolId, string userId)
		{
			var currentUser = !string.IsNullOrEmpty(userId) ? new Guid(userId) : Guid.Empty;
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.MineshaftHashrateChart(poolId, currentUser), TimeSpan.FromMinutes(5), async () =>
			{
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var chartData = new HashrateChartModel();
					var pool = await context.Pool.FirstOrDefaultNoLockAsync(x => x.Id == poolId).ConfigureAwait(false);

					var statInterval = 30;
					var finish = DateTime.UtcNow;
					var start = finish.AddHours(-24);
					var totalChartPoints = (1440 * 1) / statInterval;

					var shareData = await context.Database.SqlQuery<HashrateChartData>(string.Format(_query, pool.TablePrefix), start).ToListNoLockAsync().ConfigureAwait(false);
					if (shareData.IsNullOrEmpty())
						return chartData;

					var userShareData = new List<HashrateChartData>();
					if (currentUser != Guid.Empty)
						userShareData = await context.Database.SqlQuery<HashrateChartData>(string.Format(_userQuery, pool.TablePrefix), currentUser, start).ToListNoLockAsync().ConfigureAwait(false);

					for (int i = 0; i < totalChartPoints; i++)
					{
						var rangeStart = start.AddMinutes(i * statInterval);
						var rangeEnd = rangeStart.AddMinutes(statInterval);
						var data = shareData.FirstOrDefault(x => x.Timestamp > rangeStart && x.Timestamp <= rangeEnd);
						var javaTime = rangeStart.ToJavaTime();

						var statPeriod = i == totalChartPoints
							? (data.LastTime - rangeStart).TotalMinutes
							: statInterval * 60;

						var hashrate = data != null ? PoolExtensions.CalculateHashrate(data.Shares, pool.AlgoType, statPeriod) : 0.0;
						chartData.PoolData.Add(new[] { javaTime, hashrate });

						if (currentUser != Guid.Empty)
						{
							var userData = userShareData.FirstOrDefault(x => x.Timestamp > rangeStart && x.Timestamp <= rangeEnd);
							var userHashrate = userData != null ? PoolExtensions.CalculateHashrate(userData.Shares, pool.AlgoType, statPeriod) : 0.0;
							chartData.UserData.Add(new[] { javaTime, userHashrate });
						}
					}
					return chartData;
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<BlockChartModel> GetBlockChart(int poolId)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.MineshaftBlockChart(poolId), TimeSpan.FromSeconds(30), async () =>
			{
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var chartData = new BlockChartModel();
					var blocks = await context.Blocks
							.AsNoTracking()
							.Where(x => x.PoolId == poolId && x.IsProcessed)
							.OrderByDescending(x => x.Height)
							.Take(25)
							.Select(x => new
							{
								Height = x.Height,
								Shares = x.Shares,
								EstimatedShares = x.EstimatedShares
							}).ToListNoLockAsync().ConfigureAwait(false);
					foreach (var block in blocks.OrderBy(x => x.Height))
					{
						chartData.BlockHeightData.Add(block.Height);
						chartData.ActualData.Add(block.Shares);
						chartData.EstimatedData.Add(block.EstimatedShares);
					}
					return chartData;
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}


		public async Task<DataTablesResponse> GetMiners(int poolId, DataTablesModel param)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.MineshaftMiners(poolId), TimeSpan.FromSeconds(10), async () =>
			{
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var query = await context.UserStatistics
						.AsNoTracking()
						.Where(x => x.PoolId == poolId && x.Hashrate > 0)
						.Select(x => new
						{
							Name = x.User.MiningHandle,
							Hashrate = x.Hashrate
						})
						.OrderByDescending(x => x.Hashrate)
						.ToListNoLockAsync().ConfigureAwait(false);

					var results = query.Select((x, i) => new
					{
						Rank = i + 1,
						Name = x.Name,
						Hashrate = x.Hashrate
					}).OrderBy(x => x.Rank);

					return results.GetDataTableResult(param, true);
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}
	}
}
