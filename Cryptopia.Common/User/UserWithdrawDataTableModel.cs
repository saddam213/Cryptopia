using Cryptopia.Enums;
using System;

namespace Cryptopia.Common.User
{
	public class UserWithdrawDataTableModel
	{
		public int Id { get; set; }
		public string Symbol { get; set; }
		public decimal Amount { get; set; }
		public decimal Fee { get; set; }
		public WithdrawStatus Status { get; set; }
		public string TransactionId { get; set; }
		public string Address { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}