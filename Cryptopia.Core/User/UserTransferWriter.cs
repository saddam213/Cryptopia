using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.DataContext;

namespace Cryptopia.Core.User
{
	public class UserTransferWriter : IUserTransferWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }
	}
}