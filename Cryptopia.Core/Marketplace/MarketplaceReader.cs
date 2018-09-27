using Cryptopia.Base.Extensions;
using Cryptopia.Common.Cache;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.Marketplace;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptopia.Core.Marketplace
{
	public class MarketplaceReader : IMarketplaceReader
	{
		public ICacheService CacheService { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<List<MarketCategoryModel>> GetMarketCategories()
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.MarketCategories(), TimeSpan.FromHours(1), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var query = context.MarketCategory
					.AsNoTracking()
					.Select(x => new MarketCategoryModel
					{
						Id = x.Id,
						ParentId = x.ParentId,
						Name = x.Name,
						DisplayName = x.DisplayName,
						ItemCount = (from marketItem in context.MarketItem.Where(n => n.Status == MarketItemStatus.Active)
												 join level1 in context.MarketCategory on marketItem.CategoryId equals level1.Id
												 join level2 in context.MarketCategory on level1.ParentId equals level2.Id
												 join level3 in context.MarketCategory on level2.ParentId equals level3.Id
												 where x.Id == level1.Id || x.Id == level2.Id || x.Id == level3.Id
												 select 1).Count()
					});
					return await query.ToListNoLockAsync().ConfigureAwait(false);
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<List<MarketListItemModel>> GetMarketItems(MarketItemSearchModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var pageNum = model.Page;
				var categoryId = model.CategoryId;
				var locationId = model.LocationId;
				var currencyId = model.CurrencyId;
				var searchTerm = model.SearchTerm ?? "";
				var sortby = model.SortBy ?? "Title";
				var marketItemType = model.ItemType;
				var query = from marketItem in context.MarketItem.Where(n => n.Status == MarketItemStatus.Active)
										join category1 in context.MarketCategory on marketItem.CategoryId equals category1.Id
										join category2 in context.MarketCategory on category1.ParentId equals category2.Id
										join category3 in context.MarketCategory on category2.ParentId equals category3.Id
										join location1 in context.Location on marketItem.LocationId equals location1.Id
										join location2 in context.Location on location1.ParentId equals location2.Id
										where (marketItemType == null || marketItem.Type == marketItemType)
										&& (currencyId == 0 || currencyId == marketItem.CurrencyId)
										&& (categoryId == 0 || (categoryId == category1.Id || categoryId == category2.Id || categoryId == category3.Id))
										&& (locationId == 0 || (locationId == location1.Id || locationId == location2.Id))
										&& (searchTerm == "" || (marketItem.Title.Contains(searchTerm) || marketItem.Description.Contains(searchTerm)))
										select new
										{
											Id = marketItem.Id,
											Category = marketItem.Category.DisplayName,
											Location = marketItem.Location.ParentId != null
												? string.Concat(marketItem.Location.Parent.Name, ", ", marketItem.Location.Name)
												: marketItem.Location.Name,
											Title = marketItem.Title,
											Description = marketItem.Description,
											MainImage = marketItem.MainImage,
											AskingPrice = marketItem.AskingPrice,
											ReservePrice = marketItem.ReservePrice,
											CurrentBidPrice = (decimal?)marketItem.Bids.Max(x => x.BidAmount) ?? 0,
											Type = marketItem.Type,
											Status = marketItem.Status,
											Feature = marketItem.Feature,
											CloseDate = marketItem.CloseDate,
											Created = marketItem.Created,
											Symbol = marketItem.Currency.Symbol,
											IsAdult = (category1.Id == Constant.MARKETPLACE_CATEGORY_ADULT || category2.Id == Constant.MARKETPLACE_CATEGORY_ADULT || category3.Id == Constant.MARKETPLACE_CATEGORY_ADULT)
										};

				switch (sortby)
				{
					case "Title":
						query = query.OrderBy(x => x.Title);
						break;
					case "Highest Price":
						query = query.OrderByDescending(x => x.AskingPrice);
						break;
					case "Lowest Price":
						query = query.OrderBy(x => x.AskingPrice);
						break;
					case "Closing Soon":
						query = query.OrderBy(x => x.CloseDate);
						break;
					case "Newest":
						query = query.OrderByDescending(x => x.Created);
						break;
					default:
						break;
				}

				var results = await query.ToListNoLockAsync().ConfigureAwait(false);
				return results.Select(marketItem => new MarketListItemModel
				{
					Title = marketItem.Title,
					Closes = marketItem.CloseDate.GetTimeToGo(),
					Featured = marketItem.Feature,
					Id = marketItem.Id,
					ItemType = marketItem.Type,
					Location = marketItem.Location,
					MainImage = marketItem.MainImage,
					NoReserve = marketItem.ReservePrice > 0,
					Price = marketItem.AskingPrice,
					ReserveMet = marketItem.CurrentBidPrice >= marketItem.ReservePrice,
					Symbol = marketItem.Symbol,
					IsAdult = marketItem.IsAdult
				}).ToList();
			}
		}


		public async Task<MarketplaceItemModel> GetMarketItem(int marketItemId)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.MarketItem
					.Where(x => x.Id == marketItemId)
					.Select(marketItem => new
					{
						User = marketItem.User,
						CategoryId = marketItem.CategoryId,
						Id = marketItem.Id,
						CloseDate = marketItem.CloseDate,
						Created = marketItem.Created,
						AskingPrice = marketItem.AskingPrice,
						Description = marketItem.Description,
						Feature = marketItem.Feature,
						LocationId = marketItem.LocationId,
						Location = marketItem.Location.ParentId != null
											? string.Concat(marketItem.Location.Parent.Name, ", ", marketItem.Location.Name)
											: marketItem.Location.Name,
						ReservePrice = marketItem.ReservePrice,
						MainImage = marketItem.MainImage,
						Status = marketItem.Status,
						Title = marketItem.Title,
						Type = marketItem.Type,
						CurrencyId = marketItem.CurrencyId,
						Symbol = marketItem.Currency.Symbol,
						LocationRegion = marketItem.LocationRegion,
						AllowPickup = marketItem.AllowPickup,
						PickupOnly = marketItem.PickupOnly,
						ShippingBuyerArrange = marketItem.ShippingBuyerArrange,
						ShippingNational = marketItem.ShippingNational,
						ShippingInternational = marketItem.ShippingInternational,
						ShippingNationalPrice = marketItem.ShippingNationalPrice,
						ShippingNationalDetails = marketItem.ShippingNationalDetails,
						ShippingInternationalPrice = marketItem.ShippingInternationalPrice,
						ShippingInternationalDetails = marketItem.ShippingInternationalDetails,
						AlternateImages = marketItem.Images.Select(x => x.Image).ToList(),
						Questions = marketItem.Questions.Select(q => new MarketQuestionModel
						{
							Id = q.Id,
							Answer = q.Answer,
							Question = q.Question,
							Timestamp = q.Timestamp,
							UserName = q.User.UserName
						}).ToList(),
						Bids = marketItem.Bids.Select(b => new MarketItemBidModel
						{
							Id = b.Id,
							Amount = b.BidAmount,
							IsWinningBid = b.IsWinningBid,
							Timestamp = b.Timestamp,
							TrustRating = b.User.TrustRating,
							UserId = b.UserId,
							UserName = b.User.UserName
						}).ToList(),
						HasBuyersFeedback = marketItem.Feedback.Any(f => f.ReceiverUserId == marketItem.UserId),
						HasSellersFeedback = marketItem.Feedback.Any(f => f.SenderUserId == marketItem.UserId),
						Buyer = marketItem.Bids.Any(x => x.IsWinningBid)
							? marketItem.Bids.FirstOrDefault(x => x.IsWinningBid).User
							: null,
						
					});
				var result = await query.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (result == null)
					return null;

				var marketItemModel = new MarketplaceItemModel
				{
					UserId = result.User.Id,
					UserName = result.User.UserName,
					UserTrustRating = result.User.TrustRating,
					CategoryId = result.CategoryId,
					Id = result.Id,
					CloseDate = result.CloseDate,
					Created = result.Created,
					AskingPrice = result.AskingPrice,
					Description = result.Description,
					Feature = result.Feature,
					LocationId = result.LocationId,
					Location = result.Location,
					LocationRegion = result.LocationRegion,
					ReservePrice = result.ReservePrice,
					MainImage = result.MainImage,
					Status = result.Status,
					Title = result.Title,
					Type = result.Type,
					CurrencyId = result.CurrencyId,
					Symbol = result.Symbol,
					AllowPickup = result.AllowPickup,
					PickupOnly = result.PickupOnly,
					ShippingBuyerArrange = result.ShippingBuyerArrange,
					ShippingNational = result.ShippingNational,
					ShippingInternational = result.ShippingInternational,
					ShippingNationalPrice = result.ShippingNationalPrice,
					ShippingNationalDetails = result.ShippingNationalDetails,
					ShippingInternationalPrice = result.ShippingInternationalPrice,
					ShippingInternationalDetails = result.ShippingInternationalDetails,
					AlternateImages = result.AlternateImages,
					Questions = result.Questions,
					Bids = result.Bids.OrderByDescending(b => b.Amount).ToList(),
					HasBuyersFeedback = result.HasBuyersFeedback,
					HasSellersFeedback = result.HasSellersFeedback,
					BuyerUserId = result.Buyer == null ? null : new Guid?(result.Buyer.Id),
					BuyerUserName = result.Buyer == null ? string.Empty : result.Buyer.UserName,
					BuyerTrustRating = result.Buyer == null ? 0 : result.Buyer.TrustRating,
					CurrentBidPrice = result.Bids.Any() ? result.Bids.Max(x => x.Amount) : 0
				};

				return marketItemModel;
			}
		}


		public async Task<List<MarketplaceFeedbackModel>> GetUserFeedback(string username)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.MarketFeedback
				.AsNoTracking()
				.Where(x => x.ReceiverUser.UserName == username)
				.OrderByDescending(x => x.Timestamp)
				.Select(x => new MarketplaceFeedbackModel
				{
					MarketItemId = x.MarketItemId,
					Comment = x.Comment,
					Rating = x.Rating,
					Receiver = x.ReceiverUser.UserName,
					Sender = x.SenderUser.UserName,
					Timestamp = x.Timestamp
				});

				return await query.ToListNoLockAsync().ConfigureAwait(false);
			}
		}
	}
}
