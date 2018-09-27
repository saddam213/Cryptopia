using System.Collections.Generic;

namespace Cryptopia.Common.User
{
	public class UserNotificationModel
	{
		public UserNotificationModel()
		{
			Notifications = new List<UserNotificationItemModel>();
		}
		public List<UserNotificationItemModel> Notifications { get; set; }
	}
}
