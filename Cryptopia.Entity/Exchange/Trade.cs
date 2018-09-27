using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cryptopia.Enums;

namespace Cryptopia.Entity
{
	public class Trade
	{
		[Key]
		public int Id { get; set; }

		public Guid UserId { get; set; }
		public int TradePairId { get; set; }

		[Column("TradeTypeId")]
		public TradeHistoryType Type { get; set; }

		public decimal Amount { get; set; }
		public decimal Rate { get; set; }
		public decimal Fee { get; set; }
		public DateTime Timestamp { get; set; }

		[Column("TradeStatusId")]
		public TradeStatus Status { get; set; }

		public decimal Remaining { get; set; }
		public bool IsApi { get; set; }


		[ForeignKey("UserId")]
		public virtual Entity.User User { get; set; }

		[ForeignKey("TradePairId")]
		public virtual Entity.TradePair TradePair { get; set; }
	}
}