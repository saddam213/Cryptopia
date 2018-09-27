using Cryptopia.Common.Cache;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.Mineshaft;
using Cryptopia.Common.Pool;
using Cryptopia.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.Mineshaft
{
	public class MineshaftWriter : IMineshaftWriter
	{
		public ICacheService CacheService { get; set; }
		public IPoolReader PoolReader { get; set; }
		public IPoolDataContextFactory PoolDataContextFactory { get; set; }

		public async Task<IWriterResult> ChangeUserPool(string userId, ChangePoolModel model)
		{
			var currentUser = new Guid(userId);
			using (var context = PoolDataContextFactory.CreateContext())
			{
				var pool = await PoolReader.GetPool(model.PoolId).ConfigureAwait(false);
				if(pool.Status != PoolStatus.OK)
					return new WriterResult(false, $"Pool #{model.PoolId} not found.");

				var userWorkers = await context.Worker.Where(x => x.UserId == currentUser && x.AlgoType == pool.AlgoType && x.IsEnabled).ToListNoLockAsync().ConfigureAwait(false);
				foreach (var worker in userWorkers)
				{
					if (model.AllWorkers || model.SelectedWorkers.Contains(worker.Id))
						worker.TargetPool = pool.Symbol;
				}
				await context.SaveChangesAsync().ConfigureAwait(false);

				var message = model.AllWorkers
					? $"Successfully moved all workers to the {pool.Name}({pool.Symbol}) pool."
					: $"Successfully moved selected workers to the {pool.Name}({pool.Symbol}) pool.";
				return new WriterResult(true, message);
			}
		}
	}
}
