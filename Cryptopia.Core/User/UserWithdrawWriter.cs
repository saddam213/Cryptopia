using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.DataContext;

namespace Cryptopia.Core.User
{
	public class UserWithdrawWriter : IUserWithdrawWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }
	}
}
