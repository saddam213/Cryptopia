using Cryptopia.Enums;
using System;
using System.Collections.Generic;

namespace Cryptopia.Common.Marketplace
{
	public class MarketplaceItemModel
	{
		public int Id { get; set; }
		public Guid UserId { get; set; }
		public string UserName { get; set; }
		public int CategoryId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string MainImage { get; set; }
		public int CurrencyId { get; set; }
		public string Symbol { get; set; }
		public decimal AskingPrice { get; set; }
		public decimal ReservePrice { get; set; }
		public decimal CurrentBidPrice { get; set; }
		public MarketItemType Type { get; set; }
		public MarketItemStatus Status { get; set; }
		public MarketItemFeature Feature { get; set; }
		public DateTime CloseDate { get; set; }
		public DateTime Created { get; set; }
		public List<MarketQuestionModel> Questions { get; set; }
		public List<MarketItemBidModel> Bids { get; set; }
		public List<string> AlternateImages { get; set; }
		public int LocationId { get; set; }
		public string Location { get; set; }
		public string LocationRegion { get; set; }
		public bool AllowPickup { get; set; }
		public bool PickupOnly { get; set; }
		public bool ShippingBuyerArrange { get; set; }
		public bool ShippingNational { get; set; }
		public bool ShippingInternational { get; set; }
		public decimal ShippingNationalPrice { get; set; }
		public string ShippingNationalDetails { get; set; }
		public decimal ShippingInternationalPrice { get; set; }
		public string ShippingInternationalDetails { get; set; }
		public bool HasSellersFeedback { get; set; }
		public bool HasBuyersFeedback { get; set; }
		public Guid? BuyerUserId { get; set; }
		public double UserTrustRating { get; set; }
		public string BuyerUserName { get; set; }
		public double BuyerTrustRating { get; set; }
	}
}
