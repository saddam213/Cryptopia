using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Enums;

namespace Cryptopia.Common.TradeNotification
{
	public class TradePriceUpdate : ITradeDataUpdate
	{
		public int TradePairId { get; set; }
		public string Market { get; set; }
		public double Change { get; set; }
		public decimal Last { get; set; }
		public decimal High { get; set; }
		public decimal Low { get; set; }
		public decimal Volume { get; set; }
		public decimal BaseVolume { get; set; }

		public Guid? UserId { get; set; }
		public TradeDataType DataType
		{
			get { return TradeDataType.Price; }
		}
	}
}
