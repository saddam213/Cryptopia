using System.Threading.Tasks;

namespace Cryptopia.Common.User
{
	public interface IUserSettingsReader
	{
		Task<UserSettingsModel> GetSettings(string userId);
	}
}