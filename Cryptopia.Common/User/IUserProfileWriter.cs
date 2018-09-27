using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.User
{
	public interface IUserProfileWriter
	{
		Task<WriterResult> UpdateProfile(string userId, UserProfileModel model);
	}
}
