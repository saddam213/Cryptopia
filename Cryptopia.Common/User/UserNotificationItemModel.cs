using System;

namespace Cryptopia.Common.User
{
	public class UserNotificationItemModel
	{
		public string Notification { get; set; }
		public DateTime Timestamp { get; set; }
		public string Title { get; set; }
		public string Type { get; set; }
		public int Id { get; set; }
	}
}
