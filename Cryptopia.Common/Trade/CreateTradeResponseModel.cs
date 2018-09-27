using Cryptopia.Common.Notifications;
using Cryptopia.Common.TradeNotification;
using System.Collections.Generic;

namespace Cryptopia.Common.Trade
{
	public class CreateTradeResponseModel
	{
		public string Error { get; set; }
		public bool IsError
		{
			get { return !string.IsNullOrEmpty(Error); }
		}

		public int? OrderId { get; set; }
		public List<int> FillerOrders { get; set; }
		
		public List<INotification> Notifications { get; set; }
		public List<ITradeDataUpdate> DataUpdates { get; set; }
	}
}