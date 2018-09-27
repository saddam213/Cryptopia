using System.Threading.Tasks;

namespace Cryptopia.Common.Notifications
{
	public interface INotificationService
	{
		Task<bool> SendNotification(NotificationModel notification);
		Task<bool> SendDataNotification(DataNotificationModel notification);
		Task<bool> CreateNotification(CreateNotificationModel notification);
		Task<bool> ClearNotifications(string userId);
		Task<bool> DeleteNotifications(string userId);
	}
}