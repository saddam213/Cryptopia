using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Cryptopia.Entity
{
	public class MarketItemBid
	{
		[Key]
		public int Id { get; set; }
		public Guid UserId { get; set; }
		public int MarketItemId { get; set; }
		public decimal BidAmount { get; set; }
		public bool IsWinningBid { get; set; }
		public DateTime Timestamp { get; set; }


		[ForeignKey("UserId")]
		public virtual Entity.User User { get; set; }

		[ForeignKey("MarketItemId")]
		public virtual Entity.MarketItem MarketItem { get; set; }
	}
}
