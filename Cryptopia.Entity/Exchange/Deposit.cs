using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cryptopia.Enums;

namespace Cryptopia.Entity
{
	public class Deposit
	{
		[Key]
		public int Id { get; set; }

		public Guid UserId { get; set; }
		public int CurrencyId { get; set; }
		public decimal Amount { get; set; }
		public string Txid { get; set; }
		public int Confirmations { get; set; }

		[Column("DepositTypeId")]
		public DepositType Type { get; set; }

		[Column("DepositStatusId")]
		public DepositStatus Status { get; set; }

		public DateTime TimeStamp { get; set; }

		[ForeignKey("UserId")]
		public virtual Entity.User User { get; set; }

		[ForeignKey("CurrencyId")]
		public virtual Currency Currency { get; set; }
	}
}