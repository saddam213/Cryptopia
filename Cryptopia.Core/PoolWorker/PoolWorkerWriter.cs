using Cryptopia.Common.Cache;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.PoolWorker;
using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.PoolWorker
{
	public class PoolWorkerWriter : IPoolWorkerWriter
	{
		public ICacheService CacheService { get; set; }
		public IPoolDataContextFactory PoolDataContextFactory { get; set; }

		public async Task<IWriterResult> CreateWorker(string userId, PoolWorkerCreateModel model)
		{
			try
			{
				var currentUserId = new Guid(userId);
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var algoConnection =
						await context.Connection.FirstOrDefaultNoLockAsync(x => x.AlgoType == model.AlgoType).ConfigureAwait(false);
					if (algoConnection == null)
						return new WriterResult(false, "Invalid algorithm.");

					var worker = await context.Worker
						.Where(w => w.UserId == currentUserId && w.Name == model.FullName && w.AlgoType == model.AlgoType)
						.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (worker != null && worker.IsEnabled)
						return new WriterResult(false, "Worker '{0}' already exists", model.Name);

					if (worker == null)
					{
						worker = new Entity.PoolWorker
						{
							Name = model.FullName,
							UserId = currentUserId,
							AlgoType = model.AlgoType.Value,
						};
						context.Worker.Add(worker);
					}

					worker.TargetPool = algoConnection.DefaultPool;
					if (model.IsAutoSwitch)
					{
						worker.TargetPool = await context.Statistics
							.Where(
								x =>
									x.Pool.IsEnabled && x.Pool.AlgoType == model.AlgoType &&
									(x.Pool.Status == Enums.PoolStatus.OK || x.Pool.Status == Enums.PoolStatus.Expiring))
							.OrderByDescending(x => x.Profitability)
							.Select(x => x.Pool.Symbol)
							.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					}

					worker.Password = model.Password;
					worker.IsAutoSwitch = model.IsAutoSwitch;
					worker.TargetDifficulty = model.TargetDifficulty;
					worker.IsEnabled = true;
					await context.SaveChangesAsync().ConfigureAwait(false);
					await CacheService.InvalidateAsync(CacheKey.PoolWorkers(userId)).ConfigureAwait(false);
					return new WriterResult(true, $"Successfully created worker '{worker.Name}'");
				}
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}

		public async Task<IWriterResult> DeleteWorker(string userId, int workerId)
		{
			try
			{
				var currentUserId = new Guid(userId);
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var worker = await context.Worker
						.Where(w => w.Id == workerId && w.UserId == currentUserId)
						.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (worker == null)
						return new WriterResult(true);

					worker.IsEnabled = false;
					await context.SaveChangesAsync().ConfigureAwait(false);
					await CacheService.InvalidateAsync(CacheKey.PoolWorkers(userId)).ConfigureAwait(false);
					return new WriterResult(true, $"Successfully deleted worker '{worker.Name}'");
				}
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}

		public async Task<IWriterResult> UpdateWorker(string userId, PoolWorkerUpdateModel model)
		{
			var currentUserId = new Guid(userId);
			using (var context = PoolDataContextFactory.CreateContext())
			{
				var worker =
					await
						context.Worker.Where(w => w.Id == model.Id && w.UserId == currentUserId)
							.FirstOrDefaultNoLockAsync()
							.ConfigureAwait(false);
				if (worker == null)
					return new WriterResult(false, "Worker '{0}' not found.", model.Name);

				var profitPool = string.Empty;
				if (model.IsAutoSwitch)
				{
					worker.TargetPool = await context.Statistics
						.Where(
							x =>
								x.Pool.IsEnabled && x.Pool.AlgoType == model.AlgoType &&
								(x.Pool.Status == Enums.PoolStatus.OK || x.Pool.Status == Enums.PoolStatus.Expiring))
						.OrderByDescending(x => x.Profitability)
						.Select(x => x.Pool.Symbol)
						.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				}

				worker.Password = model.Password;
				worker.IsAutoSwitch = model.IsAutoSwitch;
				worker.TargetDifficulty = model.TargetDifficulty;
				await context.SaveChangesAsync().ConfigureAwait(false);
				await CacheService.InvalidateAsync(CacheKey.PoolWorkers(userId)).ConfigureAwait(false);
				return new WriterResult(true, $"Successfully updated worker '{worker.Name}'");
			}
		}

		public async Task<IWriterResult> UpdateWorkerPool(string userId, PoolWorkerUpdatePoolModel model)
		{
			var currentUserId = new Guid(userId);
			using (var context = PoolDataContextFactory.CreateContext())
			{
				var worker =
					await
						context.Worker.Where(w => w.Id == model.Id && w.UserId == currentUserId)
							.FirstOrDefaultNoLockAsync()
							.ConfigureAwait(false);
				if (worker == null)
					return new WriterResult(false, "Worker '{0}' not found.", model.Name);

				worker.TargetPool = model.TargetPool;
				worker.IsAutoSwitch = false;
				await context.SaveChangesAsync().ConfigureAwait(false);
				await CacheService.InvalidateAsync(CacheKey.PoolWorkers(userId)).ConfigureAwait(false);
				return new WriterResult(true, $"Successfully changed pool for worker '{worker.Name}'");
			}
		}

		public async Task<IWriterResult> AdminUpdateWorker(AdminPoolWorkerUpdateModel model)
		{
			using (var context = PoolDataContextFactory.CreateContext())
			{
				var worker = await context.Worker.Where(w => w.Id == model.Id).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (worker == null)
					return new WriterResult(false, "Worker '{0}' not found.", model.Name);

				worker.Password = model.Password;
				worker.IsAutoSwitch = model.IsAutoSwitch;
				worker.TargetDifficulty = model.TargetDifficulty;
				worker.TargetPool = model.TargetPool;
				await context.SaveChangesAsync().ConfigureAwait(false);
				await CacheService.InvalidateAsync(CacheKey.PoolWorkers(worker.UserId.ToString())).ConfigureAwait(false);
				return new WriterResult(true, $"Successfully updated worker '{worker.Name}'");
			}
		}

		public async Task<IWriterResult> AdminUpdateWorkerPool(AdminUpdateWorkerPoolModel model)
		{
			try
			{
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var pool = await context.Pool
						.Where(
							c => c.Id == model.PoolId && (c.Status == Enums.PoolStatus.Maintenance || c.Status == Enums.PoolStatus.Offline))
						.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (pool == null)
						return new WriterResult(false, "Pool not found or is offline");

					var workers =
						await context.Worker.Where(x => x.AlgoType == pool.AlgoType).ToListNoLockAsync().ConfigureAwait(false);
					foreach (var worker in workers)
					{
						worker.TargetPool = pool.Symbol;
					}

					await context.SaveChangesAsync().ConfigureAwait(false);
					return new WriterResult(true, $"Successfully moved all {pool.AlgoType} to the {pool.Symbol} pool.");
				}
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}