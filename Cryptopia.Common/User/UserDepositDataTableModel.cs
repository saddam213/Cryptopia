using Cryptopia.Enums;
using System;

namespace Cryptopia.Common.User
{
	public class UserDepositDataTableModel
	{
		public int Id { get; set; }
		public string Symbol { get; set; }
		public decimal Amount { get; set; }
		public DepositStatus Status { get; set; }
		public DepositType Type { get; set; }
		public string TransactionId { get; set; }
		public string Confirmations { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}
