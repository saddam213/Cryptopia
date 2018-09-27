using Cryptopia.Enums;
using System;

namespace Cryptopia.Common.TermDeposits
{
	public class UpdateTermDepositPaymentModel
	{
		public int PaymentId { get; set; }
		public string Name { get; set; }
		public string UserName { get; set; }
		public Guid UserId { get; set; }
		public decimal Amount { get; set; }
		public TermDepositPaymentType Type { get; set; }
		public string TransactionId { get; set; }
		public string Address { get; set; }
	}
}
