using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cryptopia.Enums;

namespace Cryptopia.Entity
{
	public class TradePair
	{
		[Key]
		public int Id { get; set; }

		public int CurrencyId1 { get; set; }
		public int CurrencyId2 { get; set; }
		public decimal LastTrade { get; set; }
		public double Change { get; set; }

		[Column("StatusId")]
		public TradePairStatus Status { get; set; }

		public string StatusMessage { get; set; }


		[ForeignKey("CurrencyId1")]
		public virtual Currency Currency1 { get; set; }

		[ForeignKey("CurrencyId2")]
		public virtual Currency Currency2 { get; set; }
	}
}