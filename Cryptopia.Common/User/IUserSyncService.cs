using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.User
{
	public interface IUserSyncService
	{
		Task<IServiceResult> SyncUser(string userId);
	}
}
