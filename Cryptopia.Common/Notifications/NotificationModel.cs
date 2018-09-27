using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Notifications
{
	public class NotificationModel : INotification
	{
		public string Header { get; set; }
		public string Notification { get; set; }
		public NotificationLevelType Type { get; set; }
		public Guid? UserId { get; set; }
	
}
}
