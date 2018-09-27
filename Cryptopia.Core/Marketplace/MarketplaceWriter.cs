using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.Marketplace;
using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using Cryptopia.Base;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.Marketplace
{
	public class MarketplaceWriter : IMarketplaceWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<IWriterResult<int>> CreateMarketItem(string userId, CreateMarketModel model)
		{
			try
			{
				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == currentUser);
					if (user == null)
						return new WriterResult<int>(false, "Unauthorized");

					var newItem = new Entity.MarketItem
					{
						CategoryId = model.CategoryId,
						UserId = user.Id,
						Title = model.Title,
						Description = model.Description,
						CurrencyId = model.CurrencyId,
						AskingPrice = model.AskingPrice,
						ReservePrice = model.ReservePrice,
						CloseDate = model.CloseDate,
						Created = DateTime.UtcNow,
						LocationId = model.LocationId,
						MainImage = model.MainImage,
						Type = model.Type,
						Feature = MarketItemFeature.Normal, //request.Feature;
						Status = MarketItemStatus.Active,
						LocationRegion = model.LocationRegion ?? string.Empty,
						AllowPickup = model.AllowPickup,
						PickupOnly = model.PickupOnly,
						ShippingBuyerArrange = model.ShippingBuyerArrange,
						ShippingNational = model.ShippingNational,
						ShippingInternational = model.ShippingInternational,
						ShippingNationalPrice = model.ShippingNationalPrice,
						ShippingNationalDetails = model.ShippingNationalDetails,
						ShippingInternationalPrice = model.ShippingInternationalPrice,
						ShippingInternationalDetails = model.ShippingInternationalDetails,
						Images = new List<Entity.MarketItemImage>()
					};

					if (model.AlternateImages.Any())
					{
						foreach (var image in model.AlternateImages.Where(x => !string.IsNullOrEmpty(x)))
						{
							newItem.Images.Add(new Entity.MarketItemImage {Image = image});
						}
					}

					context.MarketItem.Add(newItem);
					await context.SaveChangesAsync().ConfigureAwait(false);
					return new WriterResult<int>(true, newItem.Id);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}


		public async Task<IWriterResult> UpdateMarketItem(string userId, UpdateMarketModel model)
		{
			try
			{
				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == currentUser);
					if (user == null)
						return new WriterResult(false, "Unauthorized");

					var marketItem = await context.MarketItem
						.Include(m => m.Bids)
						.Include(m => m.Images)
						.Where(x => x.UserId == currentUser && x.Id == model.Id)
						.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (marketItem == null)
						return new WriterResult(false, $"Market item #{model.Id} not found.");

					if ((marketItem.Type == MarketItemType.Auction && !marketItem.Bids.IsNullOrEmpty()) ||
					    (marketItem.Type == MarketItemType.BuySell || marketItem.Type == MarketItemType.Wanted))
					{
						marketItem.Status = MarketItemStatus.Active;
						marketItem.CurrencyId = model.CurrencyId;
						marketItem.AskingPrice = model.AskingPrice;
						marketItem.ReservePrice = model.ReservePrice;
					}

					marketItem.CategoryId = model.CategoryId;
					marketItem.Title = model.Title;
					marketItem.Description = model.Description;
					marketItem.LocationId = model.LocationId;
					marketItem.MainImage = model.MainImage;
					marketItem.Feature = MarketItemFeature.Normal; //request.Feature;
					marketItem.LocationRegion = model.LocationRegion ?? string.Empty;
					marketItem.AllowPickup = model.AllowPickup;
					marketItem.PickupOnly = model.PickupOnly;
					marketItem.ShippingBuyerArrange = model.ShippingBuyerArrange;
					marketItem.ShippingNational = model.ShippingNational;
					marketItem.ShippingInternational = model.ShippingInternational;
					marketItem.ShippingNationalPrice = model.ShippingNationalPrice;
					marketItem.ShippingNationalDetails = model.ShippingNationalDetails;
					marketItem.ShippingInternationalPrice = model.ShippingInternationalPrice;
					marketItem.ShippingInternationalDetails = model.ShippingInternationalDetails;

					if (model.IsRelist)
					{
						marketItem.CloseDate = model.CloseDate;
					}

					context.MarketItemImage.RemoveRange(marketItem.Images);
					if (model.AlternateImages.Any())
					{
						foreach (var image in model.AlternateImages.Where(x => !string.IsNullOrEmpty(x)))
						{
							context.MarketItemImage.Add(new Entity.MarketItemImage
							{
								MarketItemId = marketItem.Id,
								Image = image
							});
						}
					}

					await context.SaveChangesAsync().ConfigureAwait(false);
					return new WriterResult(true);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}


		public async Task<IWriterResult> RelistMarketItem(string userId, int marketItemId)
		{
			try
			{
				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == currentUser);
					if (user == null)
						return new WriterResult(false, "Unauthorized");

					var marketItem = await context.MarketItem
						.Include(m => m.Images)
						.Where(x => x.UserId == currentUser && x.Id == marketItemId)
						.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (marketItem == null)
						return new WriterResult(false, $"Market item #{marketItemId} not found.");

					return await CreateMarketItem(userId, new CreateMarketModel
					{
						AllowPickup = marketItem.AllowPickup,
						AlternateImages = marketItem.Images.Select(x => x.Image).ToList(),
						AskingPrice = marketItem.AskingPrice,
						CategoryId = marketItem.CategoryId,
						CloseDate = marketItem.CloseDate,
						CurrencyId = marketItem.CurrencyId,
						Description = marketItem.Description,
						LocationId = marketItem.LocationId,
						LocationRegion = marketItem.LocationRegion,
						MainImage = marketItem.MainImage,
						PickupOnly = marketItem.PickupOnly,
						ReservePrice = marketItem.ReservePrice,
						ShippingBuyerArrange = marketItem.ShippingBuyerArrange,
						ShippingInternational = marketItem.ShippingInternational,
						ShippingInternationalDetails = marketItem.ShippingInternationalDetails,
						ShippingInternationalPrice = marketItem.ShippingInternationalPrice,
						ShippingNational = marketItem.ShippingNational,
						ShippingNationalDetails = marketItem.ShippingNationalDetails,
						ShippingNationalPrice = marketItem.ShippingNationalPrice,
						Title = marketItem.Title,
						Type = marketItem.Type
					});
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<IWriterResult> CancelMarketItem(string userId, int marketItemId)
		{
			try
			{
				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == currentUser);
					if (user == null)
						return new WriterResult(false, "Unauthorized");

					var marketItem = await context.MarketItem
						.Where(x => x.UserId == currentUser && x.Id == marketItemId)
						.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (marketItem == null)
						return new WriterResult(false, $"Market item #{marketItemId} not found.");

					marketItem.Status = MarketItemStatus.Canceled;
					await context.SaveChangesAsync().ConfigureAwait(false);
					return new WriterResult(true);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}


		public async Task<IWriterResult> CreateQuestion(string userId, CreateMarketQuestionModel model)
		{
			try
			{
				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == currentUser);
					if (user == null)
						return new WriterResult(false, "Unauthorized");

					var marketItem = await context.MarketItem
						.Include(m => m.Questions)
						.Where(x => x.Id == model.MarketItemId)
						.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (marketItem == null)
						return new WriterResult(false, $"Market item #{model.MarketItemId} not found.");

					marketItem.Questions.Add(new Entity.MarketItemQuestion
					{
						Answer = "",
						Question = model.Question,
						Timestamp = DateTime.UtcNow,
						UserId = currentUser
					});

					await context.SaveChangesAsync().ConfigureAwait(false);
					model.MarketItemUserId = marketItem.UserId;
					return new WriterResult(true);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}


		public async Task<IWriterResult> CreateAnswer(string userId, CreateMarketAnswerModel model)
		{
			try
			{
				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == currentUser);
					if (user == null)
						return new WriterResult(false, "Unauthorized");

					var question = await context.MarketItemQuestion
						.Where(x => x.Id == model.QuestionId && x.MarketItem.UserId == currentUser)
						.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (question == null)
						return new WriterResult(false, $"Question #{model.QuestionId} not found.");

					question.Answer = model.Answer;
					await context.SaveChangesAsync().ConfigureAwait(false);
					model.QuestionUserId = question.UserId;
					return new WriterResult(true);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<IWriterResult> CreateBid(string userId, CreateMarketBidModel model)
		{
			try
			{
				if (model.BidAmount < 0.00000001M)
					return new WriterResult(false, "Your bid must be greater than 0.00000001");

				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == currentUser);
					if (user == null)
						return new WriterResult(false, "Unauthorized");

					var marketItem = await context.MarketItem
						.Include(m => m.Bids)
						.Where(x => x.Id == model.MarketItemId)
						.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (marketItem == null)
						return new WriterResult(false, $"Market item #{model.MarketItemId} not found.");

					if (marketItem.Status != MarketItemStatus.Active)
						return new WriterResult(false, "This item has closed");

					var maxBid = marketItem.Bids.Any() ? marketItem.Bids.Max(x => x.BidAmount) : 0;
					if (maxBid >= model.BidAmount)
						return new WriterResult(false, $"Your bid must be greater than {maxBid}.");

					var newbid = new Entity.MarketItemBid
					{
						BidAmount = marketItem.Type == MarketItemType.Auction
							? model.BidAmount
							: marketItem.AskingPrice,
						MarketItemId = marketItem.Id,
						Timestamp = DateTime.UtcNow,
						UserId = currentUser
					};

					if (marketItem.Type != MarketItemType.Auction)
					{
						newbid.IsWinningBid = true;
						marketItem.Status = MarketItemStatus.Complete;
					}

					marketItem.Bids.Add(newbid);
					await context.SaveChangesAsync().ConfigureAwait(false);
					model.MarketItemUserId = marketItem.UserId;
					return new WriterResult(true);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<IWriterResult> CreateFeedback(string userId, CreateMarketFeedbackModel model)
		{
			try
			{
				var trustRating = 0d;
				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == currentUser);
					if (user == null)
						return new WriterResult(false, "Unauthorized");

					var marketItem = await context.MarketItem
						.Include(m => m.User)
						.Include(m => m.Bids)
						.Include(m => m.Feedback)
						.Where(x => x.Id == model.MarketItemId && x.Status == MarketItemStatus.Complete)
						.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (marketItem == null)
						return new WriterResult(false, $"Market item #{model.MarketItemId} not found.");

					var winningBid = marketItem.Bids.FirstOrDefault(x => x.IsWinningBid);
					if (winningBid == null)
						return new WriterResult(false, "Could not find buyer");

					var receivingUser = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == winningBid.UserId);
					if (receivingUser == null)
						return new WriterResult(false, "Could not find user");

					var receiver = marketItem.UserId == user.Id
						? receivingUser.Id
						: marketItem.UserId;

					var existing = marketItem.Feedback.FirstOrDefault(x => x.ReceiverUserId == receiver);
					if (existing != null)
						return new WriterResult(false, "Feedback has already been placed for this user.");

					var feedback = new Entity.MarketFeedback
					{
						Comment = model.Comment,
						Rating = Math.Min(Math.Max(model.Rating, 0), 5),
						SenderUserId = user.Id,
						ReceiverUserId = receiver,
						MarketItemId = marketItem.Id,
						Timestamp = DateTime.UtcNow
					};

					context.MarketFeedback.Add(feedback);
					await context.SaveChangesAsync().ConfigureAwait(false);

					var receiversFeedback =
						await context.MarketFeedback.Where(x => x.ReceiverUserId == receiver).ToListNoLockAsync().ConfigureAwait(false);
					if (receiversFeedback.Any())
					{
						trustRating = Math.Min(
							Math.Floor(((double) receiversFeedback.Sum(x => x.Rating)/receiversFeedback.Count)/0.5)*0.5, 5);
					}

					model.Receiver = receiver;
				}

				using (var context = DataContextFactory.CreateContext())
				{
					var receiverId = model.Receiver.ToString();
					var receivingUser = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == receiverId);
					if (receivingUser != null)
					{
						receivingUser.TrustRating = trustRating;
						await context.SaveChangesAsync().ConfigureAwait(false);
					}
				}

				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var receivingUser = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == model.Receiver);
					if (receivingUser != null)
					{
						receivingUser.TrustRating = trustRating;
						await context.SaveChangesAsync().ConfigureAwait(false);
					}
				}

				return new WriterResult(true);
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}