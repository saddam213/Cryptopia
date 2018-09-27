using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.User
{
	public interface IUserNotificationWriter
	{
		Task<IWriterResult> Delete(string userId);
		Task<IWriterResult> Clear(string userId);
	}
}