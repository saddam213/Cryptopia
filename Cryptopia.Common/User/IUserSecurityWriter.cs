using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.User
{
	public interface IUserSecurityWriter
	{
		Task<IWriterResult> UpdateApiSettings(string userId, UpdateApiModel model);
		Task<IWriterResult> UpdateWithdrawSettings(string userId, UpdateWithdrawModel model);
	}
}
