using Cryptopia.Common.Notifications;
using Cryptopia.Common.TradeNotification;
using System.Collections.Generic;

namespace Cryptopia.Common.Trade
{
	public class CreateTipResponseModel
	{
		public string Error { get; set; }
		public bool IsError
		{
			get { return !string.IsNullOrEmpty(Error); }
		}
		public List<INotification> Notifications { get; set; }
	}
}