using Cryptopia.Common.MineshaftNotification;
using Cryptopia.Enums;
using Cryptopia.PoolService.TradeService;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Cryptopia.PoolService.Implementation
{
	public class PoolNotifier : IDisposable
	{

		private const string NotificationProxyName = "NotificationHub";
		private const string ProxyName = "MineshaftHub";
		private readonly string _connectionUrl = ConfigurationManager.AppSettings["ClientNotificationUrl"];

		public PoolNotifier()
		{
			ServicePointManager.DefaultConnectionLimit = 10000;
		}



		public async Task<bool> SendNotifications(List<TradeNotification> notifications)
		{
			using (var connection = new HubConnection(_connectionUrl))
			{
				var proxy = connection.CreateHubProxy(NotificationProxyName);
				if (proxy == null)
					return false;

				await connection.Start();
				foreach (var notification in notifications)
				{
					await proxy.Invoke("OnNotification", notification.Type.ToString(), notification.UserId.ToString(), notification.Header, notification.Notification);
				}
				return true;
			}
		}


		public async Task<bool> SendDataNotification(IMineshaftNotification notification)
		{
			return await SendDataNotifications(new List<IMineshaftNotification> { notification });
		}

		public async Task<bool> SendDataNotifications(List<IMineshaftNotification> notifications)
		{
			var notificationsMessages = notifications.Select(x => new MineshaftNotification
			{
				UserId = x.UserId,
				Data = x,
				Type = x.Type,
				PoolId = x.PoolId
			}).ToList();
			using (var connection = new HubConnection(_connectionUrl))
			{
				var proxy = connection.CreateHubProxy(ProxyName);
				if (proxy == null)
					return false;

				await connection.Start();
				await proxy.Invoke("SendNotification", notificationsMessages);
				return true;
			}
		}


		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
