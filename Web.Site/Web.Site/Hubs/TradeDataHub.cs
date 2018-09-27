using Cryptopia.Common.Trade;
using Cryptopia.Common.TradeNotification;
using Cryptopia.Common.TradePair;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Site.Notifications
{
	public class TradeDataHub : Hub
	{

	}

	public static class TradeDataHubExtensions
	{
		public static async Task SendTradeNotifications(this List<ITradeDataUpdate> dataUpdates)
		{
			if (dataUpdates == null || !dataUpdates.Any())
				return;

			var hubContext = GlobalHost.ConnectionManager.GetHubContext<TradeDataHub>();
			if (hubContext == null)
				return;

			var tasks = new List<Task>();
			foreach (var dataUpdate in dataUpdates)
			{
				if (!dataUpdate.UserId.HasValue)
				{
					tasks.Add(hubContext.Clients.All.SendTradeDataUpdate(dataUpdate));
					continue;
				}
				tasks.Add(hubContext.Clients.User(dataUpdate.UserId.ToString()).SendUserTradeDataUpdate(dataUpdate));
			}
			await Task.WhenAll(tasks);
		}
	}

}