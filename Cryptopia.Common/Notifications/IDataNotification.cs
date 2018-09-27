using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Notifications
{
	public interface IDataNotification
	{
		Guid? UserId { get; set; }
		object Data { get; set; }
		string Event { get; }
		DataNotificationType Type { get; set; }
	}
}
