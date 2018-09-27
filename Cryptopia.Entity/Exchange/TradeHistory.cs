using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cryptopia.Enums;

namespace Cryptopia.Entity
{
	public class TradeHistory
	{
		[Key]
		public int Id { get; set; }

		public Guid UserId { get; set; }
		public Guid ToUserId { get; set; }
		public int TradePairId { get; set; }
		public int CurrencyId { get; set; }

		[Column("TradeHistoryTypeId")]
		public TradeHistoryType Type { get; set; }

		public decimal Amount { get; set; }
		public decimal Rate { get; set; }
		public decimal Fee { get; set; }
		public DateTime Timestamp { get; set; }
		public bool IsApi { get; set; }


		[ForeignKey("UserId")]
		public virtual Entity.User User { get; set; }

		[ForeignKey("ToUserId")]
		public virtual Entity.User ToUser { get; set; }

		[ForeignKey("TradePairId")]
		public virtual TradePair TradePair { get; set; }
	}
}