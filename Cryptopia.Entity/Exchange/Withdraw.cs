using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cryptopia.Enums;

namespace Cryptopia.Entity
{
	public class Withdraw
	{
		[Key]
		public int Id { get; set; }

		public Guid UserId { get; set; }
		public int CurrencyId { get; set; }

		[MaxLength(256)]
		public string Address { get; set; }

		public decimal Amount { get; set; }
		public decimal Fee { get; set; }
		public int Confirmations { get; set; }

		[MaxLength(256)]
		public string Txid { get; set; }

		[Column("WithdrawTypeId")]
		public WithdrawType Type { get; set; }

		[Column("WithdrawStatusId")]
		public WithdrawStatus Status { get; set; }

		[MaxLength(2000)]
		public string TwoFactorToken { get; set; }

		public decimal EstimatedPrice { get; set; }
		public DateTime? Confirmed { get; set; }
		public DateTime TimeStamp { get; set; }
		public bool IsApi { get; set; }

		[ForeignKey("UserId")]
		public virtual Entity.User User { get; set; }

		[ForeignKey("CurrencyId")]
		public virtual Currency Currency { get; set; }

        public int RetryCount { get; set; }
    }
}