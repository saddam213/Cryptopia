using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Web.Site.Cache;
using Cryptopia.Enums;
using Cryptopia.Common.Currency;
using Cryptopia.Common.Location;
using Cryptopia.Common.Marketplace;
using Cryptopia.Common.Validation;

namespace Web.Site.Models
{
	public class MarketPlaceModel
	{
		private MarketCategoryModel _category;
		private CurrencyModel _currency;
		private LocationModel _location;

		public MarketPlaceModel()
		{
			Categories = new List<MarketCategoryModel>();
			SortBys = new List<string> { 
				Resources.Market.marketItemSortyByTitle, 
				Resources.Market.marketItemSortyByNewest, 
				Resources.Market.marketItemSortyByHighestPrice, 
				Resources.Market.marketItemSortyByLowestPrice,
				Resources.Market.marketItemSortyByClosingSoon
			};
			ItemTypes = new List<string> { 
				Cryptopia.Resources.Market.itemTypeAll,
				Cryptopia.Resources.Market.itemTypeBuySell,
				Cryptopia.Resources.Market.itemTypeAuction,
				Cryptopia.Resources.Market.itemTypeWanted
			};
		}

		public int CategoryId { get; set; }
		public string SortBy { get; set; }
		public string SearchTerm { get; set; }
		public int LocationId { get; set; }
		public int CurrencyId { get; set; }
		public string ItemType { get; set; }

		public List<MarketCategoryModel> Categories { get; set; }
		public IPagedList<MarketListItemModel> MarketItems { get; set; }
		public MarketItemModel MarketItem { get; set; }


		public bool IsItemMode
		{
			get { return MarketItem != null; }
		}

		public MarketCategoryModel Category
		{
			get
			{
				if (_category == null)
				{
					_category = Categories.FirstOrDefault(x => x.Id == CategoryId);
				}
				return _category;
			}
		}


		public CurrencyModel Currency
		{
			get
			{
				if (_currency == null)
				{
					_currency = Currencies.FirstOrDefault(x => x.CurrencyId == CurrencyId) ??
					            new CurrencyModel { CurrencyId = 0, Name = "All Currencies"};
				}
				return _currency;
			}
		}

		public LocationModel Location
		{
			get
			{
				if (_location == null)
				{
					_location = Locations.FirstOrDefault(x => x.Id == LocationId) ??
					            new LocationModel {Id = 0, Name = "All Locations"};
				}
				return _location;
			}
		}

		public IEnumerable<MarketCategoryModel> BreadCrumbs()
		{
			if (Category.ParentId > 0)
			{
				var parent = Categories.FirstOrDefault(x => x.Id == Category.ParentId);
				if (parent != null && parent.ParentId > 0)
				{
					yield return Categories.FirstOrDefault(x => x.Id == parent.ParentId);
				}
				yield return parent;
			}
			yield return Category;
		}

		public List<LocationModel> Locations { get; set; }
		public List<CurrencyModel> Currencies { get; set; }


		public List<string> SortBys { get; set; }
		public List<string> ItemTypes { get; set; }
	}



	public class MarketItemModel
	{
		public int Id { get; set; }
		public bool IsUserListing { get; set; }
		public string UserName { get; set; }
		public int CategoryId { get; set; }
		public string Location { get; set; }
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

		public bool IsUserBuyer { get; set; }

		public double SellersTrustRating { get; set; }


		public double BuyersTrustRating { get; set; }

		public string BuyerUserName { get; set; }


		public bool IsAuction { get; set; }
		public bool ReserveMet { get; set; }
		public bool NoReserve { get; set; }
	}


	public class CreateMarketItemModel
	{
		public CreateMarketItemModel()
		{
			AuctionDuration = 14;
			Currencies = new List<CurrencyModel>();
			Categories = new List<MarketCategoryModel>();
		}

		[RequiredBase()]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Market.createItemTitleLabel), ResourceType = typeof(Resources.Market))]
		public string Title { get; set; }

		[RequiredBase()]
		[StringLengthBase(2500)]
		[Display(Name = nameof(Resources.Market.createItemDescriptionLabel), ResourceType = typeof(Resources.Market))]
		public string Description { get; set; }

		//[Display(Name = "Main Image")]
		//[HttpPostedFileExtension(ErrorMessage = "Image must be one of the following image formats: {1}")]
		//public HttpPostedFileBase MainImage { get; set; }

		[RequiredBase()]
		[Display(Name = nameof(Resources.Market.createItemSellPriceLabel), ResourceType = typeof(Resources.Market))]
		public decimal Price { get; set; }

		// [RequiredIf("Type", MarketItemType.Auction, ErrorMessage = "Please set a reserve price.")]
		[Display(Name = nameof(Resources.Market.createItemReservePriceLabel), ResourceType = typeof(Resources.Market))]
		public decimal ReservePrice { get; set; }

		[RequiredBase()]
		[Display(Name = nameof(Resources.Market.createItemCurrencyLabel), ResourceType = typeof(Resources.Market))]
		public int CurrencyId { get; set; }

		[RegularExpression(@"^[0-9]\d*$", ErrorMessageResourceName = nameof(Resources.Market.createItemCategoryRequiredError), ErrorMessageResourceType = typeof(Resources.Market))]
		[Display(Name = nameof(Resources.Market.createItemMainCategoryLabel), ResourceType = typeof(Resources.Market))]
		public int MainCategoryId { get; set; }

		[RegularExpression(@"^[0-9]\d*$", ErrorMessageResourceName = nameof(Resources.Market.createItemCategoryRequiredError), ErrorMessageResourceType = typeof(Resources.Market))]
		[Display(Name = nameof(Resources.Market.createItemCategoryLabel), ResourceType = typeof(Resources.Market))]
		public int? CategoryId { get; set; }

		[RegularExpression(@"^[0-9]\d*$", ErrorMessageResourceName = nameof(Resources.Market.createItemCategoryRequiredError), ErrorMessageResourceType = typeof(Resources.Market))]
		[Display(Name = nameof(Resources.Market.createItemSubCategoryLabel), ResourceType = typeof(Resources.Market))]
		public int? SubCategoryId { get; set; }

		[Display(Name = nameof(Resources.Market.createItemTypeLabel), ResourceType = typeof(Resources.Market))]
		public MarketItemType Type { get; set; }

		//[Display(Name = "Alternate Images")]
		//public IEnumerable<HttpPostedFileBase> AlternateImages { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingAllowedPickupLabel), ResourceType = typeof(Resources.Market))]
		public bool AllowPickup { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingPickupOnlyLabel), ResourceType = typeof(Resources.Market))]
		public bool PickupOnly { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingByBuyerLabel), ResourceType = typeof(Resources.Market))]
		public bool ShippingBuyerArrange { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingNationalLabel), ResourceType = typeof(Resources.Market))]
		public bool ShippingNational { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingPriceLabel), ResourceType = typeof(Resources.Market))]
		public decimal ShippingNationalPrice { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingDetailsLabel), ResourceType = typeof(Resources.Market))]
		public string ShippingNationalDetails { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingInternationalLabel), ResourceType = typeof(Resources.Market))]
		public bool ShippingInternational { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingPriceLabel), ResourceType = typeof(Resources.Market))]
		public decimal ShippingInternationalPrice { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingDetailsLabel), ResourceType = typeof(Resources.Market))]
		public string ShippingInternationalDetails { get; set; }

		[RequiredBase()]
		[Display(Name = nameof(Resources.Market.createItemCountryLabel), ResourceType = typeof(Resources.Market))]
		public int? CountryId { get; set; }

		[RegularExpression(@"^[0-9]\d*$", ErrorMessageResourceName = nameof(Cryptopia.Resources.Validation.attributeRequiredError), ErrorMessageResourceType = typeof(Cryptopia.Resources.Validation))]
		[Display(Name = nameof(Resources.Market.createItemStateCityLabel), ResourceType = typeof(Resources.Market))]
		public int? CityId { get; set; }

		[Display(Name = nameof(Resources.Market.createItemRegionLabel), ResourceType = typeof(Resources.Market))]
		public string LocationRegion { get; set; }

		[HttpPostedFileExtension(ErrorMessageResourceName = nameof(Resources.Market.createItemImageFormatError), ErrorMessageResourceType = typeof(Resources.Market))]
		public HttpPostedFileBase MainImage { get; set; }

		[HttpPostedFileExtension(ErrorMessageResourceName = nameof(Resources.Market.createItemImageFormatError), ErrorMessageResourceType = typeof(Resources.Market))]
		public HttpPostedFileBase AltImage1 { get; set; }

		[HttpPostedFileExtension(ErrorMessageResourceName = nameof(Resources.Market.createItemImageFormatError), ErrorMessageResourceType = typeof(Resources.Market))]
		public HttpPostedFileBase AltImage2 { get; set; }

		[HttpPostedFileExtension(ErrorMessageResourceName = nameof(Resources.Market.createItemImageFormatError), ErrorMessageResourceType = typeof(Resources.Market))]
		public HttpPostedFileBase AltImage3 { get; set; }

		[HttpPostedFileExtension(ErrorMessageResourceName = nameof(Resources.Market.createItemImageFormatError), ErrorMessageResourceType = typeof(Resources.Market))]
		public HttpPostedFileBase AltImage4 { get; set; }
		
		[Range(3, 30, ErrorMessageResourceName = nameof(Resources.Market.createItemAuctionDurationRangeError), ErrorMessageResourceType = typeof(Resources.Market))]
		[Display(Name = nameof(Resources.Market.createItemAuctionDurationLabel), ResourceType = typeof(Resources.Market))]
		public int AuctionDuration { get; set; }

		private SelectList _currency;
		private SelectList _category;
		private SelectList _locations;

		public List<CurrencyModel> Currencies { get; set; }
		public List<MarketCategoryModel> Categories { get; set; }

		public SelectList Currency
		{
			get
			{
				if (_currency == null)
				{
					var currency = Currencies.Select(x => new SelectListItem {Value = x.CurrencyId.ToString(), Text = x.Symbol});
					_currency = new SelectList(currency, "Value", "Text");
				}
				return _currency;
			}
		}

		public SelectList Category
		{
			get
			{
				if (_category == null)
				{
					var categories = new List<SelectListItem>();
					categories.Add(new SelectListItem {Text = Cryptopia.Resources.General.PleaseSelectOption, Value = "-2"});
					foreach (var item in Categories.Where(x => x.ParentId == 0 && x.Id > 0))
					{
						categories.Add(new SelectListItem { Text = item.DisplayName, Value = item.Id.ToString() });
					}
					_category = new SelectList(categories, "Value", "Text");
				}
				return _category;
			}
		}

		public SelectList Locations
		{
			get
			{
				if (_locations == null)
				{
					var locations = new List<SelectListItem>();
					locations.Add(new SelectListItem {Text = Cryptopia.Resources.General.PleaseSelectOption, Value = null});
					foreach (var item in StaticDataCache.Locations.Where(x => x.ParentId == 0).OrderBy(x => x.Name))
					{
						locations.Add(new SelectListItem {Text = item.Name, Value = item.Id.ToString()});
					}
					_locations = new SelectList(locations, "Value", "Text");
				}
				return _locations;
			}
		}

		public int GetCategoryId()
		{
			if (SubCategoryId.HasValue && SubCategoryId > 0)
			{
				return SubCategoryId.Value;
			}
			if (CategoryId.HasValue && CategoryId > 0)
			{
				return CategoryId.Value;
			}
			return MainCategoryId;
		}

		public int GetLocationId()
		{
			if (CityId.HasValue)
			{
				return CityId.Value;
			}
			return CountryId.HasValue ? CountryId.Value : -1;
		}
	}


	public class EditMarketItemModel
	{
		public EditMarketItemModel()
		{
			AuctionDuration = 14;
			Currencies = new List<CurrencyModel>();
			Categories = new List<MarketCategoryModel>();
		}

		[RequiredBase()]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Market.createItemTitleLabel), ResourceType = typeof(Resources.Market))]
		public string Title { get; set; }

		[RequiredBase()]
		[StringLengthBase(2500)]
		[Display(Name = nameof(Resources.Market.createItemDescriptionLabel), ResourceType = typeof(Resources.Market))]
		public string Description { get; set; }

		//[Display(Name = "Main Image")]
		//[HttpPostedFileExtension(ErrorMessage = "Image must be one of the following image formats: {1}")]
		//public HttpPostedFileBase MainImage { get; set; }
		//public string CurrentMainImage { get; set; }

		[RequiredBase()]
		[Display(Name = nameof(Resources.Market.createItemSellPriceLabel), ResourceType = typeof(Resources.Market))]
		public decimal Price { get; set; }

		//   [RequiredIf("Type", MarketItemType.Auction, ErrorMessage = "Please set a reserve price.")]
		[Display(Name = nameof(Resources.Market.createItemReservePriceLabel), ResourceType = typeof(Resources.Market))]
		public decimal ReservePrice { get; set; }

		[RequiredBase()]
		[Display(Name = nameof(Resources.Market.createItemCurrencyLabel), ResourceType = typeof(Resources.Market))]
		public int CurrencyId { get; set; }

		[RegularExpression(@"^[0-9]\d*$", ErrorMessageResourceName = nameof(Resources.Market.createItemCategoryRequiredError), ErrorMessageResourceType = typeof(Resources.Market))]
		[Display(Name = nameof(Resources.Market.createItemMainCategoryLabel), ResourceType = typeof(Resources.Market))]
		public int MainCategoryId { get; set; }

		[RegularExpression(@"^[0-9]\d*$", ErrorMessageResourceName = nameof(Resources.Market.createItemCategoryRequiredError), ErrorMessageResourceType = typeof(Resources.Market))]
		[Display(Name = nameof(Resources.Market.createItemCategoryLabel), ResourceType = typeof(Resources.Market))]
		public int? CategoryId { get; set; }

		[RegularExpression(@"^[0-9]\d*$", ErrorMessageResourceName = nameof(Resources.Market.createItemCategoryRequiredError), ErrorMessageResourceType = typeof(Resources.Market))]
		[Display(Name = nameof(Resources.Market.createItemSubCategoryLabel), ResourceType = typeof(Resources.Market))]
		public int? SubCategoryId { get; set; }

		[Display(Name = nameof(Resources.Market.createItemTypeLabel), ResourceType = typeof(Resources.Market))]
		public MarketItemType Type { get; set; }

		//[Display(Name = "Alternate Images")]
		//public IEnumerable<HttpPostedFileBase> AlternateImages { get; set; }
		//public IEnumerable<string> CurrentAlternateImages { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingAllowedPickupLabel), ResourceType = typeof(Resources.Market))]
		public bool AllowPickup { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingPickupOnlyLabel), ResourceType = typeof(Resources.Market))]
		public bool PickupOnly { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingByBuyerLabel), ResourceType = typeof(Resources.Market))]
		public bool ShippingBuyerArrange { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingNationalLabel), ResourceType = typeof(Resources.Market))]
		public bool ShippingNational { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingPriceLabel), ResourceType = typeof(Resources.Market))]
		public decimal ShippingNationalPrice { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingDetailsLabel), ResourceType = typeof(Resources.Market))]
		public string ShippingNationalDetails { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingInternationalLabel), ResourceType = typeof(Resources.Market))]
		public bool ShippingInternational { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingPriceLabel), ResourceType = typeof(Resources.Market))]
		public decimal ShippingInternationalPrice { get; set; }

		[Display(Name = nameof(Resources.Market.createItemShippingDetailsLabel), ResourceType = typeof(Resources.Market))]
		public string ShippingInternationalDetails { get; set; }

		[RequiredBase()]
		[Display(Name = nameof(Resources.Market.createItemCountryLabel), ResourceType = typeof(Resources.Market))]
		public int? CountryId { get; set; }

		[RegularExpression(@"^[0-9]\d*$", ErrorMessageResourceName = nameof(Cryptopia.Resources.Validation.attributeRequiredError), ErrorMessageResourceType = typeof(Cryptopia.Resources.Validation))]
		[Display(Name = nameof(Resources.Market.createItemStateCityLabel), ResourceType = typeof(Resources.Market))]
		public int? CityId { get; set; }

		[Display(Name = nameof(Resources.Market.createItemRegionLabel), ResourceType = typeof(Resources.Market))]
		public string LocationRegion { get; set; }

		public string CurrentMainImage { get; set; }
		public string CurrentAltImage1 { get; set; }
		public string CurrentAltImage2 { get; set; }
		public string CurrentAltImage3 { get; set; }
		public string CurrentAltImage4 { get; set; }

		[HttpPostedFileExtension(ErrorMessageResourceName = nameof(Resources.Market.createItemImageFormatError), ErrorMessageResourceType = typeof(Resources.Market))]
		public HttpPostedFileBase MainImage { get; set; }

		[HttpPostedFileExtension(ErrorMessageResourceName = nameof(Resources.Market.createItemImageFormatError), ErrorMessageResourceType = typeof(Resources.Market))]
		public HttpPostedFileBase AltImage1 { get; set; }

		[HttpPostedFileExtension(ErrorMessageResourceName = nameof(Resources.Market.createItemImageFormatError), ErrorMessageResourceType = typeof(Resources.Market))]
		public HttpPostedFileBase AltImage2 { get; set; }

		[HttpPostedFileExtension(ErrorMessageResourceName = nameof(Resources.Market.createItemImageFormatError), ErrorMessageResourceType = typeof(Resources.Market))]
		public HttpPostedFileBase AltImage3 { get; set; }

		[HttpPostedFileExtension(ErrorMessageResourceName = nameof(Resources.Market.createItemImageFormatError), ErrorMessageResourceType = typeof(Resources.Market))]
		public HttpPostedFileBase AltImage4 { get; set; }

		public bool IsRelisting { get; set; }

		private SelectList _currency;
		private SelectList _category;
		private SelectList _locations;

		public List<CurrencyModel> Currencies { get; set; }
		public List<MarketCategoryModel> Categories { get; set; }

		public SelectList Currency
		{
			get
			{
				if (_currency == null)
				{
					var currency = Currencies.Select(x => new SelectListItem {Value = x.CurrencyId.ToString(), Text = x.Symbol});
					_currency = new SelectList(currency, "Value", "Text");
				}
				return _currency;
			}
		}

		public SelectList Category
		{
			get
			{
				if (_category == null)
				{
					var categories = new List<SelectListItem>();
					categories.Add(new SelectListItem { Text = Cryptopia.Resources.General.PleaseSelectOption, Value = "-2" });
					foreach (var item in Categories.Where(x => x.ParentId == 0 && x.Id > 0))
					{
						categories.Add(new SelectListItem { Text = item.DisplayName, Value = item.Id.ToString() });
					}
					_category = new SelectList(categories, "Value", "Text");
				}
				return _category;
			}
		}

		public SelectList Locations
		{
			get
			{
				if (_locations == null)
				{
					var locations = new List<SelectListItem>();
					locations.Add(new SelectListItem {Text = Cryptopia.Resources.General.PleaseSelectOption, Value = null});
					foreach (var item in StaticDataCache.Locations.Where(x => x.ParentId == 0).OrderBy(x => x.Name))
					{
						locations.Add(new SelectListItem {Text = item.Name, Value = item.Id.ToString()});
					}
					_locations = new SelectList(locations, "Value", "Text");
				}
				return _locations;
			}
		}

		public int GetCategoryId()
		{
			if (SubCategoryId.HasValue && SubCategoryId > 0)
			{
				return SubCategoryId.Value;
			}
			if (CategoryId.HasValue && CategoryId > 0)
			{
				return CategoryId.Value;
			}
			return MainCategoryId;
		}

		public int GetLocationId()
		{
			if (CityId.HasValue)
			{
				return CityId.Value;
			}
			return CountryId.HasValue ? CountryId.Value : -1;
		}

		public int Id { get; set; }

		public bool HasBids { get; set; }

		[Range(3, 30, ErrorMessageResourceName = nameof(Resources.Market.createItemAuctionDurationRangeError), ErrorMessageResourceType = typeof(Resources.Market))]
		[Display(Name = nameof(Resources.Market.createItemAuctionDurationLabel), ResourceType = typeof(Resources.Market))]
		public int AuctionDuration { get; set; }
	}

	public class EditMarketItemImageModel
	{
		public string CurrentImage { get; set; }
		public bool Remove { get; set; }
		public HttpPostedFileBase NewImage { get; set; }

		public bool HasImage
		{
			get { return !string.IsNullOrEmpty(CurrentImage); }
		}
	}

	public class SubmitMarketItemQuestionModel
	{
		[Required]
		public int MarketItemId { get; set; }

		[Required]
		public string Question { get; set; }
	}

	public class SubmitMarketItemBidModel
	{
		[Required]
		public int MarketItemId { get; set; }

		[Required]
		public decimal BidAmount { get; set; }

		[Required]
		public string Currency { get; set; }

		[Required]
		public string MarketItemTitle { get; set; }

		[Required]
		public MarketItemType MarketItemType { get; set; }

		public bool IsBuyNow { get; set; }
	}

	public class SubmitMarketItemAnswerModel
	{
		[Required]
		public int MarketItemId { get; set; }

		[Required]
		public int QuestionId { get; set; }

		[Required]
		public string Answer { get; set; }

		[Required]
		public string Question { get; set; }
	}

	public class MarketFeedbackModel
	{
		public string UserName { get; set; }
		public StaticPagedList<MarketplaceFeedbackModel> FeedbackItems { get; set; }
		public double TrustRating { get; set; }
		public int TotalCount { get; set; }
	}

	public class SubmitMarketItemFeedbackModel
	{
		public int MarketItemId { get; set; }
		public int Rating { get; set; }
		public string Comment { get; set; }
	}
}