using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.TradeNotification
{
	public class TradeOpenOrderUpdate : ITradeDataUpdate
	{
		public int OrderId { get; set; }
		public decimal Amount { get; set; }
		public decimal Rate { get; set; }
		public decimal Remaining { get; set; }
		public decimal Total { get; set; }
		public TradeHistoryType Type { get; set; }
		public Guid? UserId { get; set; }
		public int TradePairId { get; set; }
		public TradeStatus Status { get; set; }
		public TradeUpdateAction Action { get; set; }
		public TradeDataType DataType
		{
			get { return TradeDataType.OpenOrder; }
		}

		public string Market { get; set; }
	}
}
