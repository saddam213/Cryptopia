using Cryptopia.Common.TradeNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Enums;
using Cryptopia.Common.Notifications;

namespace Cryptopia.Common.Trade
{
	public class TipNotificationModel : INotification
	{
		public string Header { get; set; }
		public string Notification { get; set; }
		public NotificationLevelType Type { get; set; }
		public Guid? UserId { get; set; }

		public string ReceiverMessage { get; set; }
		public List<Guid> Receivers { get; set; }
	}
}
