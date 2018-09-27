using Cryptopia.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class TermDepositPayment
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public int TermDepositId { get; set; }

		[Index("IX_TransactionId", IsUnique = true)]
		public string TransactionId { get; set; }
		public decimal Amount { get; set; }
		public TermDepositPaymentType Type { get; set; }
		public DateTime Timestamp { get; set; }

		[ForeignKey("TermDepositId")]
		public virtual TermDeposit TermDeposit { get; set; }
	}
}
