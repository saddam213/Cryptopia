using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.DataContext;

namespace Cryptopia.Core.User
{
	public class UserDepositWriter : IUserDepositWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }
	}
}
