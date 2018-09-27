using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Cryptopia.Common.DataContext
{
	public interface IPoolDataContext : IDisposable
	{
		Database Database { get; }
		int SaveChanges();
		Task<int> SaveChangesAsync();

		DbSet<Entity.Pool> Pool { get; set; }
		DbSet<Entity.PoolBlock> Blocks { get; set; }
		DbSet<Entity.PoolSettings> Settings { get; set; }
		DbSet<Entity.PoolStatistics> Statistics { get; set; }

		DbSet<Entity.PoolUser> User { get; set; }
		DbSet<Entity.PoolUserPayout> UserPayout { get; set; }
		DbSet<Entity.PoolUserStatistics> UserStatistics { get; set; }

		DbSet<Entity.PoolWorker> Worker { get; set; }
		DbSet<Entity.PoolConnection> Connection { get; set; }
	}
}