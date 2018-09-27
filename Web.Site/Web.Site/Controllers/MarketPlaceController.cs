using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Site.Models;
using PagedList;
using System.Threading.Tasks;
using Web.Site.Cache;
using Microsoft.AspNet.Identity;
using Cryptopia.Enums;
using Cryptopia.Common.Currency;
using Cryptopia.Common.Marketplace;

namespace Web.Site.Controllers
{
	public class MarketPlaceController : BaseUserController
	{
		public ICurrencyReader CurrencyReader { get; set; }
		public IMarketplaceReader MarketplaceReader { get; set; }
		public IMarketplaceWriter MarketplaceWriter { get; set; }

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> Index(int? categoryId, int? currencyId, int? locationId, string sortBy, string searchTerm, int? page, MarketItemType? type)
		{
			var searchModel = new MarketItemSearchModel
			{
				CategoryId = categoryId.HasValue ? categoryId.Value : 0,
				LocationId = locationId.HasValue ? locationId.Value : 0,
				CurrencyId = currencyId.HasValue ? currencyId.Value : 0,
				SortBy = sortBy,
				ItemType = type,
				SearchTerm = searchTerm,
				Page = page.HasValue ? page.Value : 1,
				ItemsPerPage = 24 //TODO: UI selectable item count??
			};

			var response = await MarketplaceReader.GetMarketItems(searchModel);
			if (categoryId != Constant.MARKETPLACE_CATEGORY_ADULT)
				response = response.Where(x => !x.IsAdult).ToList();
		
			var pagedItems = response.Skip(searchModel.ItemsPerPage * (searchModel.Page - 1)).Take(searchModel.ItemsPerPage);
			var model = new MarketPlaceModel
			{
				CategoryId = categoryId.HasValue ? categoryId.Value : 0,
				LocationId = locationId.HasValue ? locationId.Value : 0,
				CurrencyId = currencyId.HasValue ? currencyId.Value : 0,
				ItemType = type == null ? Cryptopia.Resources.Market.itemTypeAll : type.ToString(),
				SortBy = string.IsNullOrEmpty(sortBy) ? Resources.Market.marketItemSortyByTitle : sortBy,
				SearchTerm = searchTerm,
				Categories = await MarketplaceReader.GetMarketCategories(),
				MarketItems = new StaticPagedList<MarketListItemModel>(pagedItems, Math.Max(searchModel.Page, 1), searchModel.ItemsPerPage, response.Count),
				Locations = StaticDataCache.Locations.Where(x => x.ParentId == 0).OrderBy(x => x.Name).ToList(),
				Currencies = await CurrencyReader.GetCurrencies()
			};

			return View("MarketPlace", model);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> MarketItem(int marketItemId)
		{
			var response = await MarketplaceReader.GetMarketItem(marketItemId);
			if (response == null)
				return RedirectToAction("Index");

			var currentUser = new Guid(User.Identity?.GetUserId() ?? Guid.Empty.ToString());
			response.AlternateImages.Add(response.MainImage);
			var marketItem = new MarketItemModel
			{
				IsUserListing = (User.Identity.IsAuthenticated && response.UserId == currentUser),
				IsUserBuyer = response.BuyerUserId.HasValue && (User.Identity.IsAuthenticated && response.BuyerUserId.Value == currentUser),
				Title = response.Title,
				Id = response.Id,
				CategoryId = response.CategoryId,
				Symbol = response.Symbol,
				AskingPrice = response.AskingPrice,
				Description = response.Description,
				MainImage = response.MainImage,
				AlternateImages = response.AlternateImages,
				CloseDate = response.CloseDate,
				Created = response.Created,
				CurrencyId = response.CurrencyId,
				Feature = response.Feature,
				Location = response.Location,
				Questions = response.Questions,
				ReservePrice = response.ReservePrice,
				CurrentBidPrice = response.CurrentBidPrice,
				Bids = response.Bids,
				Status = response.Status,
				Type = response.Type,
				UserName = response.UserName,
				AllowPickup = response.AllowPickup,
				PickupOnly = response.PickupOnly,
				ShippingBuyerArrange = response.ShippingBuyerArrange,
				ShippingNational = response.ShippingNational,
				ShippingInternational = response.ShippingInternational,
				ShippingNationalPrice = response.ShippingNationalPrice,
				ShippingNationalDetails = response.ShippingNationalDetails,
				ShippingInternationalPrice = response.ShippingInternationalPrice,
				ShippingInternationalDetails = response.ShippingInternationalDetails,
				HasBuyersFeedback = response.HasBuyersFeedback,
				HasSellersFeedback = response.HasSellersFeedback,
				SellersTrustRating = response.UserTrustRating,
				BuyerUserName = response.BuyerUserName,
				BuyersTrustRating = response.BuyerTrustRating,
				IsAuction = response.Type == MarketItemType.Auction,
				NoReserve = response.ReservePrice <= 0,
				ReserveMet = response.CurrentBidPrice >= response.ReservePrice,
			};

			var output = new MarketPlaceModel
			{
				CategoryId = marketItem.CategoryId,
				Categories = await MarketplaceReader.GetMarketCategories(),
				Locations = StaticDataCache.Locations.Where(x => x.ParentId == 0).OrderBy(x => x.Name).ToList(),
				Currencies = await CurrencyReader.GetCurrencies(),
				SortBy = Resources.Market.marketItemSortyByTitle,				
				MarketItem = marketItem
			};

			return View("MarketPlace", output);

		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> CreateMarketItem()
		{
			return View(new CreateMarketItemModel
			{
				Currencies = await CurrencyReader.GetCurrencies(),
				Categories = await MarketplaceReader.GetMarketCategories()
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitMarketItem(CreateMarketItemModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Currencies = await CurrencyReader.GetCurrencies();
				model.Categories = await MarketplaceReader.GetMarketCategories();
				return View("CreateMarketItem", model);
			}

			var mainImage = await model.MainImage.SaveMarketItemImageToDiskAsync(true);
			var altimage1 = await model.AltImage1.SaveMarketItemImageToDiskAsync(false);
			var altimage2 = await model.AltImage2.SaveMarketItemImageToDiskAsync(false);
			var altimage3 = await model.AltImage3.SaveMarketItemImageToDiskAsync(false);
			var altimage4 = await model.AltImage4.SaveMarketItemImageToDiskAsync(false);

			var marketItem = new CreateMarketModel
			{
				Type = model.Type,
				CategoryId = model.GetCategoryId(),
				Title = model.Title,
				CurrencyId = model.CurrencyId,
				MainImage = mainImage,
				AlternateImages = new List<string> { altimage1, altimage2, altimage3, altimage4 },
				AskingPrice = model.Price,
				ReservePrice = model.ReservePrice,
				Description = model.Description,
				LocationRegion = model.LocationRegion,
				AllowPickup = model.AllowPickup,
				PickupOnly = model.PickupOnly,
				LocationId = model.GetLocationId(),
				ShippingBuyerArrange = model.ShippingBuyerArrange,
				ShippingNational = model.ShippingNational,
				ShippingInternational = model.ShippingInternational,
				ShippingNationalPrice = model.ShippingNationalPrice,
				ShippingNationalDetails = model.ShippingNationalDetails,
				ShippingInternationalPrice = model.ShippingInternationalPrice,
				ShippingInternationalDetails = model.ShippingInternationalDetails,
				CloseDate = DateTime.UtcNow.AddDays(model.Type == MarketItemType.Auction ? Math.Max(3, model.AuctionDuration) : 14)
			};


			var result = await MarketplaceWriter.CreateMarketItem(User.Identity.GetUserId(), marketItem);
			if (!result.Success)
				return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, Resources.Market.itemActionErrorMessageTitle, Resources.Market.createItemErrorMessage));

			return RedirectToAction("MarketItem", new { marketItemId = result.Result });
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> EditMarketItem(int marketItemId, bool isRelisting = false)
		{
			var response = await MarketplaceReader.GetMarketItem(marketItemId);
			if (response == null)
				return RedirectToAction("Index");

			var currentUser = new Guid(User.Identity.GetUserId());
			if (response.UserId != currentUser)
				return RedirectToAction("MarketItem", new { marketItemId = marketItemId });

			var marketItem = new EditMarketItemModel
			{
				Title = response.Title,
				Id = response.Id,
				Type = response.Type,
				CategoryId = response.CategoryId,
				Price = response.AskingPrice,
				Description = response.Description,
				CurrentMainImage = response.MainImage,
				CurrentAltImage1 = response.AlternateImages.Take(1).FirstOrDefault(),
				CurrentAltImage2 = response.AlternateImages.Skip(1).Take(1).FirstOrDefault(),
				CurrentAltImage3 = response.AlternateImages.Skip(2).Take(1).FirstOrDefault(),
				CurrentAltImage4 = response.AlternateImages.Skip(3).Take(1).FirstOrDefault(),
				CurrencyId = response.CurrencyId,
				//   Feature = response.MarketItem.Feature,
				CityId = response.GetCityId(),
				CountryId = response.GetCountryId(),
				ReservePrice = response.ReservePrice,
				LocationRegion = response.LocationRegion,
				AllowPickup = response.AllowPickup,
				PickupOnly = response.PickupOnly,
				ShippingBuyerArrange = response.ShippingBuyerArrange,
				ShippingNational = response.ShippingNational,
				ShippingInternational = response.ShippingInternational,
				ShippingNationalPrice = response.ShippingNationalPrice,
				ShippingNationalDetails = response.ShippingNationalDetails,
				ShippingInternationalPrice = response.ShippingInternationalPrice,
				ShippingInternationalDetails = response.ShippingInternationalDetails,
				IsRelisting = isRelisting,
				HasBids = response.Bids != null && response.Bids.Any(),
				Currencies = await CurrencyReader.GetCurrencies(),
				Categories = await MarketplaceReader.GetMarketCategories()
			};

			marketItem.SetCategories(response);

			return View(marketItem);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteMarketItemImage(string imageId)
		{
			await Task.Delay(1000);
			return Json(new { IsError = false, Message = Resources.Market.createItemImageRemovedMessage });
		}


		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateMarketItem(EditMarketItemModel model)
		{
			if (!ModelState.IsValid)
			{
				return View("EditMarketItem", model);
			}

			if (string.IsNullOrEmpty(model.CurrentMainImage))
			{
				model.CurrentMainImage = await model.MainImage.SaveMarketItemImageToDiskAsync(true);
			}

			if (string.IsNullOrEmpty(model.CurrentAltImage1))
			{
				model.CurrentAltImage1 = await model.AltImage1.SaveMarketItemImageToDiskAsync(false);
			}

			if (string.IsNullOrEmpty(model.CurrentAltImage2))
			{
				model.CurrentAltImage2 = await model.AltImage2.SaveMarketItemImageToDiskAsync(false);
			}

			if (string.IsNullOrEmpty(model.CurrentAltImage3))
			{
				model.CurrentAltImage3 = await model.AltImage3.SaveMarketItemImageToDiskAsync(false);
			}

			if (string.IsNullOrEmpty(model.CurrentAltImage4))
			{
				model.CurrentAltImage4 = await model.AltImage4.SaveMarketItemImageToDiskAsync(false);
			}

			var marketItem = new UpdateMarketModel
			{
				Id = model.Id,
				// Type = MarketItemType.BuySell,
				CategoryId = model.GetCategoryId(),
				Title = model.Title,
				CurrencyId = model.CurrencyId,
				MainImage = model.CurrentMainImage,
				AlternateImages = new List<string> { model.CurrentAltImage1, model.CurrentAltImage2, model.CurrentAltImage3, model.CurrentAltImage4 },
				AskingPrice = model.Price,
				ReservePrice = model.ReservePrice,
				Description = model.Description,
				LocationRegion = model.LocationRegion,
				AllowPickup = model.AllowPickup,
				PickupOnly = model.PickupOnly,
				LocationId = model.GetLocationId(),
				ShippingBuyerArrange = model.ShippingBuyerArrange,
				ShippingNational = model.ShippingNational,
				ShippingInternational = model.ShippingInternational,
				ShippingNationalPrice = model.ShippingNationalPrice,
				ShippingNationalDetails = model.ShippingNationalDetails,
				ShippingInternationalPrice = model.ShippingInternationalPrice,
				ShippingInternationalDetails = model.ShippingInternationalDetails,
				CloseDate = DateTime.UtcNow.AddDays(model.Type == MarketItemType.Auction ? Math.Max(3, model.AuctionDuration) : 14),
				IsRelist = model.IsRelisting
			};

			var result = await MarketplaceWriter.UpdateMarketItem(User.Identity.GetUserId(), marketItem);
			if (!result.Success)
				return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, Resources.Market.itemActionErrorMessageTitle, Resources.Market.createItemUpdateErrorMessage));

			return RedirectToAction("MarketItem", new { marketItemId = model.Id });
		}


		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> RelistMarketItem(int marketItemId)
		{
			var result = await MarketplaceWriter.RelistMarketItem(User.Identity.GetUserId(), marketItemId);
			if (!result.Success)
				return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, Resources.Market.itemActionErrorMessageTitle, Resources.Market.createItemRelistErrorMessage));

			return RedirectToAction("EditMarketItem", new { marketItemId = marketItemId, isRelisting = true });
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CancelMarketItem(int marketItemId)
		{
			var result = await MarketplaceWriter.CancelMarketItem(User.Identity.GetUserId(), marketItemId);
			if (Request.IsAjaxRequest())
			{
				if (!result.Success)
					return JsonError(result.Message);

				return JsonSuccess(result.Message);
			}

			if (!result.Success)
				return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, Resources.Market.itemActionErrorMessageTitle, Resources.Market.createItemCancelErrorMessage));

			return RedirectToAction("Index");
		}


		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> MarketFeedback(string username, int? page)
		{
			var pageNum = Math.Max(page.HasValue ? page.Value : 1, 1);
			var response = await MarketplaceReader.GetUserFeedback(username);
			var output = new MarketFeedbackModel
			{
				UserName = username,
				TrustRating = response.Any() ? response.Average(x => x.Rating) : 0,
				TotalCount = response.Count,
				FeedbackItems = new StaticPagedList<MarketplaceFeedbackModel>(response.Skip(15 * (pageNum - 1)).Take(15), pageNum, 15, response.Count),
			};

			return View(output);
		}


		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitFeedback(SubmitMarketItemFeedbackModel model)
		{
			if (!ModelState.IsValid)
			{
				return RedirectToAction("MarketItem", new { marketItemId = model.MarketItemId });
			}

			var result = await MarketplaceWriter.CreateFeedback(User.Identity.GetUserId(), new CreateMarketFeedbackModel
			{
				Comment = model.Comment,
				MarketItemId = model.MarketItemId,
				Rating = model.Rating,
			});

			return RedirectToAction("MarketItem", new { marketItemId = model.MarketItemId });
		}


		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitBid(SubmitMarketItemBidModel model)
		{
			if (!ModelState.IsValid)
			{
				return RedirectToAction("MarketItem", new { marketItemId = model.MarketItemId });
			}

			var request = new CreateMarketBidModel
			{
				BidAmount = model.BidAmount,
				MarketItemId = model.MarketItemId
			};
			var result = await MarketplaceWriter.CreateBid(User.Identity.GetUserId(), request);
			if (!result.Success)
				return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, Resources.Market.itemActionErrorMessageTitle, Resources.Market.itemBidErrorMessage));

			var seller = await UserManager.FindByIdAsync(request.MarketItemUserId.ToString());
			var buyer = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			if (seller != null && buyer != null)
			{
				var callbackUrl = Url.Action("MarketItem", "MarketPlace", new { marketItemId = model.MarketItemId }, protocol: Request.Url.Scheme);
				if (model.MarketItemType == MarketItemType.BuySell)
				{
					await SendEmailAsync(EmailTemplateType.MarketPlaceNewSold, model.MarketItemId, seller.Email, seller.Id, seller.UserName, string.Format("{0} {1}", model.BidAmount, model.Currency), buyer.Email, buyer.UserName, model.MarketItemTitle, callbackUrl);
					await SendEmailAsync(EmailTemplateType.MarketPlaceBought, model.MarketItemId, buyer.Email, buyer.Id, buyer.UserName, string.Format("{0} {1}", model.BidAmount, model.Currency), seller.Email, seller.UserName, model.MarketItemTitle, callbackUrl);
				}
				else if (model.MarketItemType == MarketItemType.Auction)
				{
					await SendEmailAsync(EmailTemplateType.MarketPlaceNewBid, model.MarketItemId, seller.Email, seller.Id, seller.UserName, model.BidAmount, buyer.Email, buyer.UserName, callbackUrl);
				}
				else if (model.MarketItemType == MarketItemType.Wanted)
				{
					await SendEmailAsync(EmailTemplateType.MarketPlaceWantedAccept, model.MarketItemId, seller.Email, seller.Id, seller.UserName, string.Format("{0} {1}", model.BidAmount, model.Currency), buyer.Email, buyer.UserName, model.MarketItemTitle, callbackUrl);
					await SendEmailAsync(EmailTemplateType.MarketPlaceWantedAccepted, model.MarketItemId, buyer.Email, buyer.Id, buyer.UserName, string.Format("{0} {1}", model.BidAmount, model.Currency), seller.Email, seller.UserName, model.MarketItemTitle, callbackUrl);
				}
			}

			return RedirectToAction("MarketItem", new { marketItemId = model.MarketItemId });
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitQuestion(SubmitMarketItemQuestionModel model)
		{
			if (!ModelState.IsValid)
			{
				return RedirectToAction("MarketItem", new { marketItemId = model.MarketItemId });
			}

			var request = new CreateMarketQuestionModel
			{
				MarketItemId = model.MarketItemId,
				Question = model.Question
			};

			var result = await MarketplaceWriter.CreateQuestion(User.Identity.GetUserId(), request);
			if (!result.Success)
				return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, Resources.Market.itemActionErrorMessageTitle, Resources.Market.itemQuestionPostErrorMessage));

			var user = await UserManager.FindByIdAsync(request.MarketItemUserId.ToString());
			if (user != null)
			{
				var callbackUrl = Url.Action("MarketItem", "MarketPlace", new { marketItemId = model.MarketItemId }, protocol: Request.Url.Scheme);
				await SendEmailAsync(EmailTemplateType.MarketPlaceNewQuestion, model.MarketItemId, user.Email, user.Id, user.UserName, model.Question, callbackUrl);
			}
			return RedirectToAction("MarketItem", new { marketItemId = model.MarketItemId });
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitAnswer(SubmitMarketItemAnswerModel model)
		{
			if (!ModelState.IsValid)
			{
				return RedirectToAction("MarketItem", new { marketItemId = model.MarketItemId });
			}

			var request = new CreateMarketAnswerModel
			{
				Answer = model.Answer,
				QuestionId = model.QuestionId
			};

			var result = await MarketplaceWriter.CreateAnswer(User.Identity.GetUserId(), request);
			if (!result.Success)
				return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, Resources.Market.itemActionErrorMessageTitle, Resources.Market.itemAnswerPostErrorMessage));

			var user = await UserManager.FindByIdAsync(request.QuestionUserId.ToString());
			if (user != null)
			{
				var callbackUrl = Url.Action("MarketItem", "MarketPlace", new { marketItemId = model.MarketItemId }, protocol: Request.Url.Scheme);
				await SendEmailAsync(EmailTemplateType.MarketPlaceNewAnswer, model.MarketItemId, user.Email, user.Id, User.Identity.Name, model.Question, model.Answer, callbackUrl);
			}

			ModelState.AddModelError(Resources.Market.itemActionSuccessMessageTitle, Resources.Market.itemAnswerPostSuccessMessage);
			return RedirectToAction("MarketItem", new { marketItemId = model.MarketItemId });
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<JsonResult> GetCategories(int parentId)
		{
			var data = await MarketplaceReader.GetMarketCategories();
			var categories = new List<SelectListItem>();
			categories.Add(new SelectListItem { Text = Cryptopia.Resources.General.PleaseSelectOption, Value = "-2" });
			foreach (var item in data.Where(x => x.ParentId == parentId))
			{
				categories.Add(new SelectListItem { Text = item.DisplayName, Value = item.Id.ToString() });
			}
			return Json(new SelectList(categories, "Value", "Text"));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult GetLocations(int parentId)
		{
			var locations = new List<SelectListItem>();
			locations.Add(new SelectListItem { Text = Cryptopia.Resources.General.PleaseSelectOption, Value = "-2" });
			foreach (var item in StaticDataCache.Locations.Where(x => x.ParentId == parentId).OrderBy(x => x.Name))
			{
				locations.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
			}
			return Json(new SelectList(locations, "Value", "Text"));
		}
	}
}