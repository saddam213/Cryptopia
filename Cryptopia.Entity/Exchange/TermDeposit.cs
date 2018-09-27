using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class TermDeposit
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public int TermDepositItemId { get; set; }
		public Guid UserId { get; set; }

		[Index("IX_WithdrawId", IsUnique = true)]
		public int WithdrawId { get; set; }

		public TermDepositStatus Status { get; set; }
		public DateTime NextPayout { get; set; }
		public DateTime TermBegin { get; set; }
		public DateTime TermEnd { get; set; }
		public DateTime Closed { get; set; }



		[ForeignKey("UserId")]
		public virtual User User { get; set; }

		[ForeignKey("WithdrawId")]
		public virtual Withdraw Withdraw { get; set; }



		[ForeignKey("TermDepositItemId")]
		public virtual TermDepositItem TermDepositItem { get; set; }

		public virtual ICollection<TermDepositPayment> TermDepositPayments { get; set; }
	}
}
