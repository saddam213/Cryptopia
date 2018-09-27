using Cryptopia.Common.DataContext;

namespace IntegrationServiceTests.DataContext
{
	public class Mock_ExchangeDataContextFactory : IExchangeDataContextFactory
	{
		public IExchangeDataContext _context { get; set; }

		public IExchangeDataContext CreateContext()
		{
			return _context;
		}

		public IExchangeDataContext CreateReadOnlyContext()
		{
			return _context;
		}
	}
}
