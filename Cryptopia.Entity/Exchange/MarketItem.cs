using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cryptopia.Enums;

namespace Cryptopia.Entity
{
	public class MarketItem
	{
		[Key]
		public int Id { get; set; }

		public Guid UserId { get; set; }
		public int CategoryId { get; set; }
		public int CurrencyId { get; set; }

		[Column("MarketItemTypeId")]
		public MarketItemType Type { get; set; }

		[Column("MarketItemStatusId")]
		public MarketItemStatus Status { get; set; }

		[Column("MarketItemFeatureId")]
		public MarketItemFeature Feature { get; set; }

		[MaxLength(128)]
		public string Title { get; set; }

		[MaxLength(10000)]
		public string Description { get; set; }

		[MaxLength(256)]
		public string MainImage { get; set; }

		public decimal AskingPrice { get; set; }
		public decimal ReservePrice { get; set; }
		public int LocationId { get; set; }

		[MaxLength(128)]
		public string LocationRegion { get; set; }

		public bool AllowPickup { get; set; }
		public bool PickupOnly { get; set; }
		public bool ShippingBuyerArrange { get; set; }
		public bool ShippingNational { get; set; }
		public bool ShippingInternational { get; set; }
		public decimal ShippingNationalPrice { get; set; }

		[MaxLength(128)]
		public string ShippingNationalDetails { get; set; }

		public decimal ShippingInternationalPrice { get; set; }

		[MaxLength(128)]
		public string ShippingInternationalDetails { get; set; }

		public DateTime CloseDate { get; set; }
		public DateTime Created { get; set; }


		[ForeignKey("UserId")]
		public virtual Entity.User User { get; set; }

		[ForeignKey("CategoryId")]
		public virtual Entity.MarketCategory Category { get; set; }

		[ForeignKey("CurrencyId")]
		public virtual Entity.Currency Currency { get; set; }

		[ForeignKey("LocationId")]
		public virtual Entity.Location Location { get; set; }

		public virtual ICollection<Entity.MarketFeedback> Feedback { get; set; }

		public virtual ICollection<Entity.MarketItemBid> Bids { get; set; }

		public virtual ICollection<Entity.MarketItemImage> Images { get; set; }

		public virtual ICollection<Entity.MarketItemQuestion> Questions { get; set; }
	}
}