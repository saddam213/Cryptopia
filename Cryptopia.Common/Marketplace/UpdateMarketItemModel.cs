using System;
using System.Collections.Generic;

namespace Cryptopia.Common.Marketplace
{
	public class UpdateMarketModel
	{
		public bool AllowPickup { get; set; }
		public List<string> AlternateImages { get; set; }
		public decimal AskingPrice { get; set; }
		public int CategoryId { get; set; }
		public DateTime CloseDate { get; set; }
		public int CurrencyId { get; set; }
		public string Description { get; set; }
		public int Id { get; set; }
		public bool IsRelist { get; set; }
		public int LocationId { get; set; }
		public string LocationRegion { get; set; }
		public string MainImage { get; set; }
		public bool PickupOnly { get; set; }
		public decimal ReservePrice { get; set; }
		public bool ShippingBuyerArrange { get; set; }
		public bool ShippingInternational { get; set; }
		public string ShippingInternationalDetails { get; set; }
		public decimal ShippingInternationalPrice { get; set; }
		public bool ShippingNational { get; set; }
		public string ShippingNationalDetails { get; set; }
		public decimal ShippingNationalPrice { get; set; }
		public string Title { get; set; }
	}
}
