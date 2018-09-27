using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.DataContext;

namespace Cryptopia.Core.User
{
	public class UserExchangeWriter : IUserExchangeWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }
	}
}
