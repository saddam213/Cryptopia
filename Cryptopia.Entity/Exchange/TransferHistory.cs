using Cryptopia.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class TransferHistory
	{
		[Key]
		public int Id { get; set; }

		public Guid UserId { get; set; }
		public Guid ToUserId { get; set; }
		public int CurrencyId { get; set; }
		public TransferType TransferType { get; set; }
		public decimal Amount { get; set; }
		public decimal Fee { get; set; }
		public decimal EstimatedPrice { get; set; }
		public DateTime Timestamp { get; set; }

		[ForeignKey("UserId")]
		public virtual Entity.User User { get; set; }

		[ForeignKey("ToUserId")]
		public virtual Entity.User ToUser { get; set; }

		[ForeignKey("CurrencyId")]
		public virtual Currency Currency { get; set; }
	}
}