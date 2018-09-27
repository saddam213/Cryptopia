using System;
using Cryptopia.Infrastructure.Common.DataContext;

namespace IntegrationServiceTests.DataContext
{
	public class Mock_DataContextFactory : IDataContextFactory
	{
		public IDataContext _context { get; set; }

		public IDataContext CreateContext()
		{
			return _context;
		}

		public IDataContext CreateReadOnlyContext()
		{
			return _context;
		}
	}
}
