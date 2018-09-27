using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Notifications
{
	public class DataNotificationModel : IDataNotification
	{
		public object Data { get; set; }
		public string Event
		{
			get { return $"On{Type}"; }
		}
		public DataNotificationType Type { get; set; }

		public Guid? UserId { get; set; }
	}

	public class BalanceNotificationData
	{
		public int CurrencyId { get; set; }
		public string Symbol { get; set; }
	}

	public class TradeBalanceNotificationData
	{
		public int TradePairId { get; set; }
		public string Market { get; set; }
	}


	//public class MineshaftDataNotificationModel
	//{
	//	public object Data { get; set; }
	//	public int PoolId { get; set; }
	//	public MineshaftDataNotificationType Type { get; set; }
	//}
}
