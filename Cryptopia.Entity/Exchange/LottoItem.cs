using System;
using Cryptopia.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Cryptopia.Entity
{
	public class LottoItem
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[MaxLength(128)]
		public string Name { get; set; }

		[MaxLength(4000)]
		public string Description { get; set; }

		public int CurrencyId { get; set; }
		public int Prizes { get; set; }
		public decimal Rate { get; set; }
		public decimal Fee { get; set; }
		public int Hours { get; set; }
		public LottoType LottoType { get; set; }
		public DateTime NextDraw { get; set; }
		public decimal PrizePool { get; set; }
		public int CurrentDrawId { get; set; }
		public LottoItemStatus Status { get; set; }
		public DateTime Expires { get; set; }

		[ForeignKey("CurrencyId")]
		public virtual Currency Currency { get; set; }

		public virtual ICollection<LottoTicket> Tickets { get; set; }
	}
}