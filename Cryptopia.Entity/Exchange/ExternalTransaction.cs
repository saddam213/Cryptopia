using Cryptopia.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class ExternalTransaction
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public string ReferenceId { get; set; }
		public string ReferenceToken { get; set; }
		public Guid UserId { get; set; }
		public Guid GuarantorId { get; set; }
		public int CurrencyId1 { get; set; }
		public int CurrencyId2 { get; set; }
		public ExternalTransactionType Type { get; set; }
		public ExternalTransactionStatus Status { get; set; }
		public int EscrowInId1 { get; set; }
		public int EscrowInId2 { get; set; }
		public int EscrowOutId1 { get; set; }
		public int EscrowOutId2 { get; set; }
		public string MetaData { get; set; }
		public DateTime Updated { get; set; }
		public DateTime Created { get; set; }


		[ForeignKey("UserId")]
		public virtual Entity.User User { get; set; }

		[ForeignKey("GuarantorId")]
		public virtual Entity.User Guarantor { get; set; }

		[ForeignKey("CurrencyId1")]
		public virtual Entity.Currency Currency1 { get; set; }

		[ForeignKey("CurrencyId2")]
		public virtual Entity.Currency Currency2 { get; set; }

		[ForeignKey("EscrowInId1")]
		public virtual Entity.TransferHistory EscrowIn1 { get; set; }

		[ForeignKey("EscrowInId2")]
		public virtual Entity.TransferHistory EscrowIn2 { get; set; }

		[ForeignKey("EscrowOutId1")]
		public virtual Entity.TransferHistory EscrowOut1 { get; set; }

		[ForeignKey("EscrowOutId1")]
		public virtual Entity.TransferHistory EscrowOut2 { get; set; }

	}
}