using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Enums;

namespace Cryptopia.Common.TradeNotification
{
	public class TradeBalanceUpdate : ITradeDataUpdate
	{
		public TradeDataType DataType
		{
			get { return TradeDataType.Balance; }
		}

		public int TradePairId { get; set; }

		public Guid? UserId { get; set; }
	}
}
