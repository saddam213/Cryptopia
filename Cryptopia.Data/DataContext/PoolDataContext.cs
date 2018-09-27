using Cryptopia.Common.DataContext;
using Cryptopia.Entity;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;

namespace Cryptopia.Data.DataContext
{
	public class PoolDataContext : DbContext, IPoolDataContext
	{
		public PoolDataContext()
			: base("DefaultPoolConnection")
		{
			Database.Log = (e) => Debug.WriteLine(e);
		}

		public DbSet<Pool> Pool { get; set; }
		public DbSet<PoolBlock> Blocks { get; set; }
		public DbSet<PoolSettings> Settings { get; set; }
		public DbSet<PoolStatistics> Statistics { get; set; }

		public DbSet<PoolUser> User { get; set; }
		public DbSet<PoolUserPayout> UserPayout { get; set; }
		public DbSet<PoolUserStatistics> UserStatistics { get; set; }

		public DbSet<PoolWorker> Worker { get; set; }
		public DbSet<PoolConnection> Connection { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Add(new DecimalPropertyConvention(38, 8));

			modelBuilder.Entity<Pool>().HasRequired(p => p.Statistics).WithRequiredPrincipal();

			modelBuilder.Entity<PoolBlock>().HasRequired(p => p.User);
			modelBuilder.Entity<PoolBlock>().HasRequired(p => p.Pool).WithMany(p => p.Blocks);

			modelBuilder.Entity<PoolWorker>().HasRequired(p => p.User).WithMany(p => p.Workers);

			modelBuilder.Entity<PoolUserStatistics>().HasRequired(p => p.User).WithMany(p => p.Statistics);
			modelBuilder.Entity<PoolUserStatistics>().HasRequired(p => p.Pool).WithMany(p => p.UserStatistics);
	
			modelBuilder.Entity<PoolUserPayout>().HasRequired(p => p.User).WithMany(p => p.Payouts);
			modelBuilder.Entity<PoolUserPayout>().HasRequired(p => p.Block).WithMany(p => p.Payouts);
			modelBuilder.Entity<PoolUserPayout>().HasRequired(p => p.Pool).WithMany(p => p.UserPayouts);
		}
	}
}