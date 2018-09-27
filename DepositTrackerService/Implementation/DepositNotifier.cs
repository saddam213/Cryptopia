using Cryptopia.Enums;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;

namespace Cryptopia.DepositTrackerService.Implementation
{
	public class DepositNotifier : IDisposable
	{
		private IHubProxy _hubNotificationProxy;
		private HubConnection _hubConnection;
		private static readonly string _authToken = ConfigurationManager.AppSettings["Signalr_AuthToken"];

		public DepositNotifier()
		{
			_hubConnection = new HubConnection(ConfigurationManager.AppSettings["ClientNotificationUrl"]);
			_hubNotificationProxy = _hubConnection.CreateHubProxy("NotificationHub");
			_hubConnection.Headers.Add("auth", _authToken);
			_hubConnection.Start().Wait();
		}

		public Task SendNotification(NotificationLevelType type, Guid userId, string header, string message)
		{
			if (_hubNotificationProxy != null)
			{
				return _hubNotificationProxy.Invoke("OnNotification", type.ToString(), userId.ToString(), header, message);
			}
			return Task.FromResult(0);
		}

		public Task SendDataNotification(DataNotificationType type, Guid userId, object data)
		{
			if (_hubNotificationProxy != null)
			{
				return _hubNotificationProxy.Invoke("OnDataNotification", type, userId.ToString(), data);
			}
			return Task.FromResult(0);
		}

		public void Dispose()
		{
			if (_hubConnection != null)
			{
				_hubConnection.Dispose();
			}
			GC.SuppressFinalize(this);
		}
	}
}
