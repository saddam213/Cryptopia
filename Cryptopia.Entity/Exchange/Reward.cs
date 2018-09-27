using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class Reward
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public Guid UserId { get; set; }

		public int CurrencyId { get; set; }

		public decimal Amount { get; set; }

		public decimal Percent { get; set; }

		[MaxLength(128)]
		public string RewardType { get; set; }

		[MaxLength(256)]
		public string Message { get; set; }

		public DateTime TimeStamp { get; set; }

		[ForeignKey("UserId")]
		public virtual Entity.User User { get; set; }

		[ForeignKey("CurrencyId")]
		public virtual Currency Currency { get; set; }
	}
}
