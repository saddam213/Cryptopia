using Cryptopia.Enums;
using System;

namespace Cryptopia.Common.User
{
	public class UserTradeHistoryDataTableModel
	{
		public int Id { get; set; }
		public string Market { get; set; }
		public TradeHistoryType Type { get; set; }
		public decimal Rate { get; set; }
		public decimal Amount { get; set; }
		public decimal Total { get; set; }
		public decimal Fee { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}