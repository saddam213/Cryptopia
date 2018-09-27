using Cryptopia.Common.Notifications;
using Cryptopia.Common.TradeNotification;
using System.Collections.Generic;

namespace Cryptopia.Common.Trade
{
	public class CreateTransferResponseModel
	{
		public string Error { get; set; }
		public bool IsError
		{
			get { return !string.IsNullOrEmpty(Error); }
		}
		public List<INotification> Notifications { get; set; }
		public int TransferId { get; set; }
	}
}