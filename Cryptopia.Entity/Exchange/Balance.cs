using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class Balance
	{
		[Key]
		public int Id { get; set; }
		public Guid UserId { get; set; }
		public int CurrencyId { get; set; }
		public decimal Total { get; set; }
		public decimal Unconfirmed { get; set; }
		public decimal HeldForTrades { get; set; }
		public decimal PendingWithdraw { get; set; }
		public bool IsFavorite { get; set; }

		[ForeignKey("UserId")]
		public virtual Entity.User User { get; set; }

		[ForeignKey("CurrencyId")]
		public virtual Entity.Currency Currency { get; set; }

		[NotMapped]
		public decimal Available
		{
			get { return Total - (Unconfirmed + PendingWithdraw + HeldForTrades); }
		}
	}
}
