using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.Notifications;
using Cryptopia.Enums;
using Microsoft.AspNet.SignalR.Client;

namespace Cryptopia.Core.Notifications
{
	public class NotificationService : INotificationService
	{
		private const string ProxyName = "NotificationHub";
		private readonly string _connectionUrl = ConfigurationManager.AppSettings["ClientNotificationUrl"];
		private static readonly string _authToken = ConfigurationManager.AppSettings["Signalr_AuthToken"];

		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<bool> SendNotification(NotificationModel notification)
		{
			try
			{
				using (var connection = new HubConnection(_connectionUrl))
				{
					var proxy = connection.CreateHubProxy(ProxyName);
					if (proxy == null)
						return false;

					connection.Headers.Add("auth", _authToken);
					await connection.Start();
					await proxy.Invoke("OnNotification", notification.Type, notification.UserId, notification.Header, notification.Notification);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> SendDataNotification(DataNotificationModel notification)
		{
			try
			{
				using (var connection = new HubConnection(_connectionUrl))
				{
					var proxy = connection.CreateHubProxy(ProxyName);
					if (proxy == null)
						return false;

					connection.Headers.Add("auth", _authToken);
					await connection.Start();
					await proxy.Invoke("OnDataNotification", notification.Type, notification.UserId, notification.Data);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> CreateNotification(CreateNotificationModel notification)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var userInfo = await context.Users
						.Where(x => x.Id == notification.UserId)
						.Select(x => new Userinfo
						{
							DisableExchangeNotify = x.DisableExchangeNotify,
							DisableKarmaNotify = x.DisableKarmaNotify,
							DisableTipNotify = x.DisableTipNotify,
							DisableMarketplaceNotify = x.DisableMarketplaceNotify,
							DisablePoolNotify = x.DisablePoolNotify,
							DisableFaucetNotify = x.DisableFaucetNotify,
						}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);

					if (notification.Data != null)
					{
						await SendDataNotification(new DataNotificationModel
						{
							UserId = new Guid(notification.UserId),
							Type = notification.DataType,
							Data = notification.Data
						}).ConfigureAwait(false);
					}

					var isNotificationDisabled = IsNotificationDisabled(userInfo, notification.Type);
					if (isNotificationDisabled)
						return true;

					var entity = new Entity.UserNotification
					{
						Acknowledged = false,
						UserId = notification.UserId,
						Title = notification.Title,
						Notification = notification.Message,
						Timestamp = DateTime.UtcNow,
						Type = notification.Type.ToString()
					};
					context.Notifications.Add(entity);
					await context.SaveChangesAsync().ConfigureAwait(false);

					await SendNotification(new NotificationModel
					{
						UserId = new Guid(notification.UserId),
						Type = notification.Level,
						Header = notification.Title,
						Notification = notification.Message
					}).ConfigureAwait(false);

					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> ClearNotifications(string userId)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var notifications = await context.Notifications.Where(x => x.UserId == userId && !x.Acknowledged).ToListNoLockAsync().ConfigureAwait(false);
					if (!notifications.Any())
						return true;

					foreach (var notification in notifications)
					{
						notification.Acknowledged = true;
					}
					await context.SaveChangesAsync().ConfigureAwait(false);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> DeleteNotifications(string userId)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var notifications = await context.Notifications.Where(x => x.UserId == userId).ToListNoLockAsync().ConfigureAwait(false);
					if (!notifications.Any())
						return true;

					await context.SaveChangesAsync().ConfigureAwait(false);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		private bool IsNotificationDisabled(Userinfo userInfo, NotificationType type)
		{
			switch (type)
			{
				case NotificationType.Tip:
					return userInfo.DisableTipNotify;
				case NotificationType.Trade:
					return userInfo.DisableExchangeNotify;
				case NotificationType.Karma:
					return userInfo.DisableKarmaNotify;
				case NotificationType.Message:
					return false; //userInfo.DisableMessageNotify;
				case NotificationType.Reward:
					return userInfo.DisableFaucetNotify;
				case NotificationType.Lotto:
					return false; // userInfo.DisableFaucetNotify;
				case NotificationType.Mineshaft:
					return userInfo.DisablePoolNotify;
				case NotificationType.Marketplace:
					return userInfo.DisableMarketplaceNotify;
				default:
					return false;
			}
		}

		internal class Userinfo
		{
			public bool DisableExchangeNotify { get; set; }
			public bool DisableKarmaNotify { get; set; }
			public bool DisableTipNotify { get; set; }
			public bool DisableMarketplaceNotify { get; set; }
			public bool DisablePoolNotify { get; set; }
			public bool DisableFaucetNotify { get; set; }
		}
	}
}