using Microsoft.AspNet.SignalR.Client;
using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;

namespace Cryptopia.OutboundService.Implementation
{
	public class LottoNotifier : IDisposable
	{
		private IHubProxy _hubNotificationProxy;
		private HubConnection _hubConnection;

		public LottoNotifier()
		{
			_hubConnection = new HubConnection(ConfigurationManager.AppSettings["ClientNotificationUrl"]);
			_hubNotificationProxy = _hubConnection.CreateHubProxy("NotificationHub");
			ServicePointManager.DefaultConnectionLimit = 10000;
			_hubConnection.Start().Wait();
		}
		
		public Task SendNotification(string type, Guid userId, string header, string message)
		{
			if (_hubNotificationProxy != null)
			{
				return _hubNotificationProxy.Invoke("OnNotification", type.ToString(), userId.ToString(), header, message);
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
