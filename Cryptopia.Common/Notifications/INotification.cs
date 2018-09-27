using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Notifications
{
	public interface INotification
	{
		string Header { get; set; }
		string Notification { get; set; }
		NotificationLevelType Type { get; set; }
		Guid? UserId { get; set; }
	}
}
