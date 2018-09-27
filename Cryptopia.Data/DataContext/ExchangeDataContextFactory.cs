using Cryptopia.Common.DataContext;

namespace Cryptopia.Data.DataContext
{
	public class ExchangeDataContextFactory : IExchangeDataContextFactory
	{
		public IExchangeDataContext CreateContext()
		{
			return new ExchangeDataContext();
		}

		public IExchangeDataContext CreateReadOnlyContext()
		{
			var context = new ExchangeDataContext();
			context.Configuration.AutoDetectChangesEnabled = false;
			context.Configuration.LazyLoadingEnabled = false;
			context.Configuration.ProxyCreationEnabled = false;
			return context;
		}
	}
}