using Cryptopia.Enums;
using System;

namespace Cryptopia.Common.User
{
	public class UserTradeDataTableModel
	{
		public int Id { get; set; }
		public int TradePairId { get; set; }
		public string Market { get; set; }
		public TradeHistoryType Type { get; set; }
		public decimal Rate { get; set; }
		public decimal Amount { get; set; }
		public decimal Total { get; set; }
		public decimal Remaining { get; set; }
		public decimal Fee { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}