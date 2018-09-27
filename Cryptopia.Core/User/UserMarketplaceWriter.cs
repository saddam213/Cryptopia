using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.DataContext;

namespace Cryptopia.Core.User
{
	public class UserMarketplaceWriter : IUserMarketplaceWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }
	}
}
