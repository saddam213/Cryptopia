using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.Pool;
using System.Collections.Generic;
using Cryptopia.Enums;
using System;
using Cryptopia.Common.Cache;
using Ganss.XSS;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Core.Pool
{
	public class PoolReader : IPoolReader
	{
		public ICacheService CacheService { get; set; }
		public IPoolDataContextFactory PoolDataContextFactory { get; set; }

		public async Task<List<PoolModel>> GetPools()
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.Pools(), TimeSpan.FromSeconds(10), async () =>
			{
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var query = from pool in context.Pool
											join connection in context.Connection on pool.AlgoType equals connection.AlgoType
											where pool.IsEnabled
											select new PoolModel
											{
												Id = pool.Id,
												Name = pool.Name,
												Symbol = pool.Symbol,
												CurrencyId = pool.CurrencyId,
												AlgoType = pool.AlgoType,
												TablePrefix = pool.TablePrefix,
												Hashrate = pool.Statistics.Hashrate,
												NetworkHashrate = pool.Statistics.NetworkHashrate,
												NetworkDifficulty = pool.Statistics.NetworkDifficulty,
												FeaturedExpires = pool.FeaturedExpires,
												Expires = pool.Expires,
												Status = pool.Status,
												StatusMessage = pool.StatusMessage,
												DefaultDiff = connection.DefaultDiff,
												FixedDiffSummary = connection.FixedDiffSummary,
												VarDiffLowSummary = connection.VarDiffLowSummary,
												VarDiffHighSummary = connection.VarDiffHighSummary,
												VarDiffMediumSummary = connection.VarDiffMediumSummary
											};

					return await query.ToListNoLockAsync().ConfigureAwait(false);
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<PoolModel> GetPool(int poolId)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.Pool(poolId), TimeSpan.FromSeconds(10), async () =>
			{
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var query = from pool in context.Pool
											join connection in context.Connection on pool.AlgoType equals connection.AlgoType
											where pool.IsEnabled && pool.Id == poolId
											select new PoolModel
											{
												Id = pool.Id,
												Name = pool.Name,
												Symbol = pool.Symbol,
												CurrencyId = pool.CurrencyId,
												AlgoType = pool.AlgoType,
												TablePrefix = pool.TablePrefix,
												Hashrate = pool.Statistics.Hashrate,
												NetworkHashrate = pool.Statistics.NetworkHashrate,
												NetworkDifficulty = pool.Statistics.NetworkDifficulty,
												FeaturedExpires = pool.FeaturedExpires,
												Expires = pool.Expires,
												Status = pool.Status,
												StatusMessage = pool.StatusMessage,
												DefaultDiff = connection.DefaultDiff,
												FixedDiffSummary = connection.FixedDiffSummary,
												VarDiffLowSummary = connection.VarDiffLowSummary,
												VarDiffHighSummary = connection.VarDiffHighSummary,
												VarDiffMediumSummary = connection.VarDiffMediumSummary
											};

					return await query.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<List<PoolConnectionModel>> GetPoolConnections()
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.PoolConnections(), TimeSpan.FromSeconds(10), async () =>
			{
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var query = from connection in context.Connection
											where connection.IsEnabled
											select new PoolConnectionModel
											{
												Id = connection.Id,
												Name = connection.Name,
												AlgoType = connection.AlgoType,
												Host = connection.Host,
												Port = connection.Port,
												DefaultDiff = connection.DefaultDiff,
												FixedDiffSummary = connection.FixedDiffSummary,
												VarDiffLowSummary = connection.VarDiffLowSummary,
												VarDiffHighSummary = connection.VarDiffHighSummary,
												VarDiffMediumSummary = connection.VarDiffMediumSummary
											};

					return await query.ToListNoLockAsync().ConfigureAwait(false);
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<DataTablesResponse> GetPoolConnections(DataTablesModel model)
		{
			using (var context = PoolDataContextFactory.CreateContext())
			{
				var query = from connection in context.Connection
										where connection.IsEnabled
										select new 
										{
											Id = connection.Id,
											Name = connection.Name,
											AlgoType = connection.AlgoType,
											Host = connection.Host,
											Port = connection.Port,
											DefaultDiff = connection.DefaultDiff,
											DefaultPool = connection.DefaultPool
											//FixedDiffSummary = connection.FixedDiffSummary,
											//VarDiffLowSummary = connection.VarDiffLowSummary,
											//VarDiffHighSummary = connection.VarDiffHighSummary,
											//VarDiffMediumSummary = connection.VarDiffMediumSummary
										};

				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		public async Task<PoolConnectionModel> GetPoolConnection(AlgoType algoType)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.PoolConnection(algoType), TimeSpan.FromSeconds(10), async () =>
			{
				var sanitizer = new HtmlSanitizer();
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var query = from connection in context.Connection
											where connection.AlgoType == algoType && connection.IsEnabled
											select new PoolConnectionModel
											{
												Id = connection.Id,
												Name = connection.Name,
												AlgoType = connection.AlgoType,
												Host = connection.Host,
												Port = connection.Port,
												DefaultDiff = connection.DefaultDiff,
												DefaultPool = connection.DefaultPool,
												FixedDiffSummary = connection.FixedDiffSummary,
												VarDiffLowSummary = connection.VarDiffLowSummary,
												VarDiffHighSummary = connection.VarDiffHighSummary,
												VarDiffMediumSummary = connection.VarDiffMediumSummary
											};

					var result = await query.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					result.FixedDiffSummary = sanitizer.Sanitize(result.FixedDiffSummary);
					result.VarDiffLowSummary = sanitizer.Sanitize(result.VarDiffLowSummary);
					result.VarDiffHighSummary = sanitizer.Sanitize(result.VarDiffHighSummary);
					result.VarDiffMediumSummary = sanitizer.Sanitize(result.VarDiffMediumSummary);
					return result;
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<DataTablesResponse> GetBlocks(DataTablesModel model, int poolId)
		{
			using (var context = PoolDataContextFactory.CreateContext())
			{
				var query = context.Blocks
					.AsNoTracking()
					.Where(p => p.PoolId == poolId && p.IsProcessed)
					.OrderByDescending(x => x.Height)
					.Select(p => new
					{
						p.Height,
						p.Luck,
						p.User.MiningHandle,
						p.Amount,
						p.Difficulty,
						p.Confirmations,
						p.Status,
						p.Shares,
						p.Timestamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetPayouts(string userId, DataTablesModel model)
		{
			var currentUser = new Guid(userId);
			using (var context = PoolDataContextFactory.CreateContext())
			{
				var query = context.UserPayout
					.AsNoTracking()
					.Where(p => p.UserId == currentUser)
					.OrderByDescending(x => x.Id)
					.Select(p => new
					{
						p.Id,
						p.Pool.Symbol,
						p.Block.Height,
						p.Amount,
						p.Block.Status,
						p.Shares,
						p.TransferId,
						p.Block.Timestamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetPayouts(string userId, DataTablesModel model, int poolId)
		{
			var currentUser = new Guid(userId);
			using (var context = PoolDataContextFactory.CreateContext())
			{
				var query = context.UserPayout
					.AsNoTracking()
					.Where(p => p.UserId == currentUser && p.PoolId == poolId)
					.OrderByDescending(x => x.Id)
					.Select(p => new
					{
						p.Id,
						p.Block.Height,
						p.Amount,
						p.Block.Status,
						p.Shares,
						p.TransferId,
						p.Block.Timestamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> AdminGetPools(string userId, DataTablesModel model)
		{
			using (var context = PoolDataContextFactory.CreateContext())
			{
				var test = await context.Pool.ToListNoLockAsync().ConfigureAwait(false);
				var test1 = await context.Worker.ToListNoLockAsync().ConfigureAwait(false);
				var query = context.Pool
					.AsNoTracking()
					.Where(p => p.IsEnabled)
					.Select(pool => new
					{
						Id = pool.Id,
						Name = pool.Name,
						Symbol = pool.Symbol,
						Status = pool.Status,
						//	StatusMessage = pool.StatusMessage,
						BlockTime = pool.BlockTime,
						Expires = pool.Expires,
						FeaturedExpires = pool.FeaturedExpires,
						IsForkCheckDisabled = pool.IsForkCheckDisabled,
						WalletFee = pool.WalletFee,
						IsEnabled = pool.IsEnabled
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<AdminUpdatePoolModel> AdminGetPool(int poolId)
		{
			using (var context = PoolDataContextFactory.CreateContext())
			{
				return await context.Pool
					.AsNoTracking()
					.Where(p => p.Id == poolId)
					.Select(pool => new AdminUpdatePoolModel
					{
						Id = pool.Id,
						Pool = pool.Symbol,
						Status = pool.Status,
						StatusMessage = pool.StatusMessage,
						BlockTime = pool.BlockTime,
						IsForkCheckDisabled = pool.IsForkCheckDisabled,
						SpecialInstructions = pool.SpecialInstructions,
						WalletFee = pool.WalletFee,
						BlockReward = pool.Statistics.BlockReward
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> AdminGetPayouts(string userId, DataTablesModel model)
		{
			var currentUser = new Guid(userId);
			using (var context = PoolDataContextFactory.CreateContext())
			{
				var query = context.UserPayout
					.AsNoTracking()
					.Select(p => new
					{
						p.Id,
						p.User.UserName,
						p.Pool.Symbol,
						p.Block.Height,
						p.Amount,
						p.Block.Status,
						p.Shares,
						p.TransferId,
						p.Block.Timestamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<PoolSettingsModel> GetPoolSettings()
		{
			using (var context = PoolDataContextFactory.CreateContext())
			{
				return await context.Settings
					.AsNoTracking()
					.Select(settings => new PoolSettingsModel
					{
						ProcessorEnabled = settings.ProcessorEnabled,
						HashRateCalculationPeriod = settings.HashRateCalculationPeriod,
						StatisticsPollPeriod = settings.StatisticsPollPeriod,
						PayoutPollPeriod = settings.PayoutPollPeriod,
						SitePayoutPollPeriod = settings.SitePayoutPollPeriod,
						ProfitabilityPollPeriod = settings.ProfitabilityPollPeriod,
						ProfitSwitchEnabled = settings.ProfitSwitchEnabled,
						ProfitSwitchDepthBTC = settings.ProfitSwitchDepthBTC,
						ProfitSwitchDepthLTC = settings.ProfitSwitchDepthLTC
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
			}
		}
	}
}