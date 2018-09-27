using System;

namespace Cryptopia.Common.Marketplace
{
	public class MarketItemBidModel
	{
		public int Id { get; set; }
		public Guid UserId { get; set; }
		public bool IsWinningBid { get; set; }
		public string UserName { get; set; }
		public double TrustRating { get; set; }
		public decimal Amount { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
