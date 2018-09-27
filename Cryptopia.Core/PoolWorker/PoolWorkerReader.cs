using Cryptopia.Common.Cache;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Pool;
using Cryptopia.Common.PoolWorker;
using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;

namespace Cryptopia.Core.PoolWorker
{
	public class PoolWorkerReader : IPoolWorkerReader
	{
		public ICacheService CacheService { get; set; }
		public IPoolDataContextFactory PoolDataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetWorkers(string userId, DataTablesModel model)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.PoolWorkers(userId), TimeSpan.FromSeconds(10), async () =>
			{
				var currentUserId = new Guid(userId);
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var query = context.Worker
							.AsNoTracking()
							.Where(x => x.UserId == currentUserId && x.IsEnabled)
							.OrderBy(x => x.IsActive)
							.ThenByDescending(x => x.Hashrate)
							.Select(worker => new
							{
								Id = worker.Id,
								Name = worker.Name,
								Algo = worker.AlgoType,
								Hashrate = worker.Hashrate,
								Difficulty = worker.Difficulty,
								TargetPool = worker.TargetPool,
								TargetDifficulty = worker.TargetDifficulty,
								IsAutoSwitch = worker.IsAutoSwitch,
								IsActive = worker.IsActive
							});

					return await query.GetDataTableResultNoLockAsync(model, true).ConfigureAwait(false);
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}


		public async Task<DataTablesResponse> GetWorkers(int poolId, DataTablesModel model)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.PoolWorkers(poolId), TimeSpan.FromSeconds(10), async () =>
			{
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var pool = await context.Pool
						.AsNoTracking()
						.FirstOrDefaultNoLockAsync(x => x.Id == poolId).ConfigureAwait(false);
					if (pool == null)
						return model.GetEmptyDataTableResult();

					var query = context.Worker
							.AsNoTracking()
							.Where(x => x.AlgoType == pool.AlgoType && x.IsActive &&  x.TargetPool == pool.Symbol)
							.OrderByDescending(x => x.Hashrate)
							.Select(worker => new
							{
								Id = worker.Id,
								Name = worker.Name,
								Algo = worker.AlgoType,
								Hashrate = worker.Hashrate,
								Difficulty = worker.Difficulty,
								TargetPool = worker.TargetPool,
								TargetDifficulty = worker.TargetDifficulty,
								IsAutoSwitch = worker.IsAutoSwitch,
								IsActive = worker.IsActive
							});

					return await query.GetDataTableResultNoLockAsync(model, true);
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}


		public async Task<List<PoolWorkerModel>> GetWorkers(string userId, AlgoType algoType)
		{
			var currentUserId = new Guid(userId);
			using (var context = PoolDataContextFactory.CreateContext())
			{
				var query = context.Worker
						.AsNoTracking()
						.Where(x => x.AlgoType == algoType && x.UserId == currentUserId && x.IsEnabled)
						.OrderBy(x => x.IsActive)
						.ThenBy(x => x.Name)
						.Select(worker => new PoolWorkerModel
						{
							Id = worker.Id,
							Name = worker.Name,
							AlgoType = worker.AlgoType,
							Hashrate = worker.Hashrate,
							Difficulty = worker.Difficulty,
							TargetPool = worker.TargetPool,
							Password = worker.Password,
							TargetDifficulty = worker.TargetDifficulty,
							IsAutoSwitch = worker.IsAutoSwitch,
							IsActive = worker.IsActive
						});

				return await query.ToListNoLockAsync().ConfigureAwait(false);
			}
		}


		public async Task<PoolWorkerModel> GetWorker(string userId, int workerId)
		{
			try
			{
				var currentUserId = new Guid(userId);
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var query = context.Worker
							.AsNoTracking()
							.Where(x => x.Id == workerId && x.UserId == currentUserId && x.IsEnabled)
							.Select(worker => new PoolWorkerModel
							{
								Id = worker.Id,
								Name = worker.Name,
								AlgoType = worker.AlgoType,
								Hashrate = worker.Hashrate,
								Difficulty = worker.Difficulty,
								TargetPool = worker.TargetPool,
								Password = worker.Password,
								TargetDifficulty = worker.TargetDifficulty,
								IsAutoSwitch = worker.IsAutoSwitch,
								IsActive = worker.IsActive
							});

					return await query.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				}
			}
			catch (Exception)
			{
				return null;
			}
		}


		public async Task<DataTablesResponse> AdminGetWorkers(string userId, DataTablesModel model)
		{
			try
			{
				var currentUserId = new Guid(userId);
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var query = context.Worker
						.AsNoTracking()
						.Where(x => x.IsEnabled)
							.OrderBy(x => x.IsActive)
							.Select(worker => new
							{
								Id = worker.Id,
								Name = worker.Name,
								Algo = worker.AlgoType,
								Hashrate = worker.Hashrate,
								Difficulty = worker.Difficulty,
								TargetPool = worker.TargetPool,
								TargetDifficulty = worker.TargetDifficulty,
								IsAutoSwitch = worker.IsAutoSwitch,
								IsActive = worker.IsActive
							});

					return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
				}
			}
			catch (Exception)
			{
				return model.GetEmptyDataTableResult();
			}
		}

		public async Task<PoolWorkerModel> AdminGetWorker(string userId, int workerId)
		{
			try
			{
				var currentUserId = new Guid(userId);
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var query = context.Worker
							.AsNoTracking()
							.Where(x => x.Id == workerId && x.IsEnabled)
							.Select(worker => new PoolWorkerModel
							{
								Id = worker.Id,
								Name = worker.Name,
								AlgoType = worker.AlgoType,
								Hashrate = worker.Hashrate,
								Difficulty = worker.Difficulty,
								TargetPool = worker.TargetPool,
								Password = worker.Password,
								TargetDifficulty = worker.TargetDifficulty,
								IsAutoSwitch = worker.IsAutoSwitch,
								IsActive = worker.IsActive
							});

					return await query.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				}
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
