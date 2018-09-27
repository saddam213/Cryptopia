using Cryptopia.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class NzdtTransaction
	{
		[Key]
		public int Id { get; set; }

		public int? DepositId { get; set; }
		public Guid? UserId { get; set; }

		public NzdtTransactionStatus TransactionStatus { get; set; }

		public string Note { get; set; }

		public DateTime CreatedOn { get; set; }
		public DateTime? ProcessedOn { get; set; }

		//CSV Fields
		public DateTime Date { get; set; }
		public long UniqueId { get; set; }
		public string TranType { get; set; }
		public string ChequeNumber { get; set; }
		public string Payee { get; set; }
		public string Memo { get; set; }
		public decimal Amount { get; set; }

		[ForeignKey("DepositId")]
		public virtual Deposit Deposit { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }
	}
}
