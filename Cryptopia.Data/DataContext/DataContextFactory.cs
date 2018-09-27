using Cryptopia.Infrastructure.Common.DataContext;

namespace Cryptopia.Data.DataContext
{
	public class DataContextFactory : IDataContextFactory
	{
		public IDataContext CreateContext()
		{
			return new DataContext();
		}

		public IDataContext CreateReadOnlyContext()
		{
			var context = new DataContext();
			context.Configuration.AutoDetectChangesEnabled = false;
			context.Configuration.LazyLoadingEnabled = false;
			context.Configuration.ProxyCreationEnabled = false;
			return context;
		}
	}
}