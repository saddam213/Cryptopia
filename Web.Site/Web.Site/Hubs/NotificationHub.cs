using Cryptopia.Base;
using Cryptopia.Common.Notifications;
using Cryptopia.Common.Trade;
using Cryptopia.Common.TradeNotification;
using Cryptopia.Enums;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Site.Hubs;

namespace Web.Site.Notifications
{
	public class NotificationHub : Hub
	{
		//[AuthorizeLocal]
		public async Task OnNotification(NotificationLevelType type, string user, string header, string notification)
		{
			var notificationModel = new NotificationModel
			{
				Header = header,
				Notification = notification,
				Type = type
			};

			if (string.IsNullOrEmpty(user))
			{
				await Clients.All.SendNotification(notificationModel);
				return;
			}
			await Clients.User(user).SendNotification(notificationModel);
		}

		//[AuthorizeLocal]
		public async Task OnDataNotification(DataNotificationType type, string user, object data)
		{
			var notificationModel = new DataNotificationModel
			{
				Type = type,
				Data = data
			};

			if (string.IsNullOrEmpty(user))
			{
				await Clients.All.SendDataNotification(notificationModel);
				return;
			}
			await Clients.User(user).SendDataNotification(notificationModel);
		}
	}

	public static class NotificationHubExtensions
	{
		public static async Task SendNotifications(this List<INotification> notifications)
		{
			try
			{
				if (notifications == null || !notifications.Any())
					return;

				var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
				if (hubContext == null)
					return;

				foreach (var notification in notifications)
				{
					if (notification == null)
						return;

					if (notification is TipNotificationModel)
					{
						await SendTipNotification(hubContext, notification as TipNotificationModel);
						continue;
					}


					if (!notification.UserId.HasValue)
					{
						await hubContext.Clients.All.SendNotification(notification);

					}
					else
					{
						await hubContext.Clients.User(notification.UserId.ToString()).SendNotification(notification);
					}
					await Task.Delay(1);
				}
			}
			catch (Exception) { }
		}

		public static async Task SendDataNotifications(this List<IDataNotification> notifications)
		{
			try
			{
				if (notifications == null || !notifications.Any())
					return;

				var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
				if (hubContext == null)
					return;

				foreach (var notification in notifications)
				{
					if (notification == null)
						return;

					if (!notification.UserId.HasValue)
					{
						await hubContext.Clients.All.SendDataNotification(notification);
					}
					else
					{
						await hubContext.Clients.User(notification.UserId.ToString()).SendDataNotification(notification);
					}
					await Task.Delay(1);
				}
			
			}
			catch (Exception) { }
		}

		private static async Task SendTipNotification(IHubContext hubContext, TipNotificationModel notification)
		{
			if (notification == null || hubContext == null)
				return;


			if (!string.IsNullOrEmpty(notification.Notification))
			{
				var notificationModel = new NotificationModel
				{
					Header = "Tip Sent",
					Notification = notification.Notification,
					Type = notification.Type
				};
				await hubContext.Clients.User(notification.UserId.ToString()).SendNotification(notificationModel);
			}

			if (!string.IsNullOrEmpty(notification.ReceiverMessage) && !notification.Receivers.IsNullOrEmpty())
			{
				var receivers = notification.Receivers.Select(x => x.ToString()).ToList();
				var notificationModel = new NotificationModel
				{
					Header = "Tip Received",
					Notification = notification.ReceiverMessage,
					Type = notification.Type
				};
				await hubContext.Clients.Users(receivers).SendNotification(notificationModel);
			}
		}

		public static async Task SendTradeNotifications(this List<ITradeDataUpdate> dataUpdates)
		{
			if (dataUpdates == null || !dataUpdates.Any())
				return;

			var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
			if (hubContext == null)
				return;

			var tasks = new List<Task>();
			foreach (var dataUpdate in dataUpdates)
			{
				if (!dataUpdate.UserId.HasValue)
				{
					await hubContext.Clients.All.SendTradeDataUpdate(dataUpdate);
				}
				else
				{
					await hubContext.Clients.User(dataUpdate.UserId.ToString()).SendUserTradeDataUpdate(dataUpdate);
				}
				await Task.Delay(1);
			}
		}

	}
}