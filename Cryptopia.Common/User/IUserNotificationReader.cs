using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptopia.Common.User
{
	public interface IUserNotificationReader
	{
		Task<List<UserNotificationItemModel>> GetUserNotifications(string userId);
		Task<List<UserNotificationItemModel>> GetUserUnreadNotifications(string userId);
		Task<UserToolbarInfoModel> GetUserToolbarInfo(string v);
	}
}
