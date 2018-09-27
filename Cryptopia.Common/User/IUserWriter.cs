using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.User
{
	public interface IUserWriter
	{
		Task<IWriterResult> UpdateUser(UserModel model);
	}
}