using System.Threading.Tasks;

namespace Cryptopia.Common.User
{
	public interface IUserProfileReader
	{
		Task<UserProfileModel> GetProfile(string userId);
		Task<UserProfileInfoModel> GetProfileInfo(string userId);
	}
}
