using Cryptopia.Common.DataContext;

namespace Cryptopia.Data.DataContext
{
	public class PoolDataContextFactory : IPoolDataContextFactory
	{
		public IPoolDataContext CreateContext()
		{
			return new PoolDataContext();
		}

		public IPoolDataContext CreateReadOnlyContext()
		{
			var context = new PoolDataContext();
			context.Configuration.AutoDetectChangesEnabled = false;
			context.Configuration.LazyLoadingEnabled = false;
			context.Configuration.ProxyCreationEnabled = false;
			return context;
		}
	}
}