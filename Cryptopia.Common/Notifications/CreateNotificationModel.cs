using Cryptopia.Enums;

namespace Cryptopia.Common.Notifications
{
	public class CreateNotificationModel
	{
		public string UserId { get; set; }
		public NotificationType Type { get; set; }
		public NotificationLevelType Level { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
		public object Data { get; set; }
		public DataNotificationType DataType { get; set; }
	}
}