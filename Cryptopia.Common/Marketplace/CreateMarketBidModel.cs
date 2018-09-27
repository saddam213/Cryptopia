namespace Cryptopia.Common.Marketplace
{
	public class CreateMarketBidModel
	{
		public decimal BidAmount { get; set; }
		public int MarketItemId { get; set; }
		public object MarketItemUserId { get; set; }
	}
}
