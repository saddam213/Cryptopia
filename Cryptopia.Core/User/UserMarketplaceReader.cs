using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.DataTables;


namespace Cryptopia.Core.User
{
	public class UserMarketplaceReader : IUserMarketplaceReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetMarketItems(string userId, DataTablesModel model)
		{
			try
			{
				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var query = context.MarketItem
						.AsNoTracking()
						.Where(m => m.UserId == currentUser && m.Status == Enums.MarketItemStatus.Active)
						.Select(m => new
						{
							Id = m.Id,
							Title = m.Title,
							Type = m.Type,
							Created = m.Created,
							CloseDate = m.CloseDate,
							Status = m.Status
						});

					return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
				}
			}
			catch (Exception)
			{
				return model.GetEmptyDataTableResult();
			}
		}

		public async Task<DataTablesResponse> GetMarketHistory(string userId, DataTablesModel model)
		{
			try
			{
				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var query = from marketItem in context.MarketItem
						from winningBid in
							context.MarketItemBid.Where(b => b.MarketItemId == marketItem.Id && b.IsWinningBid).DefaultIfEmpty()
						from sellerfeedback in
							context.MarketFeedback.Where(b => b.MarketItemId == marketItem.Id && b.ReceiverUserId == currentUser)
								.DefaultIfEmpty()
						from buyerfeedback in
							context.MarketFeedback.Where(b => b.MarketItemId == marketItem.Id && b.ReceiverUserId == winningBid.UserId)
								.DefaultIfEmpty()
						where
							marketItem.Status == Enums.MarketItemStatus.Complete &&
							(marketItem.UserId == currentUser || winningBid.UserId == currentUser)
						orderby marketItem.Id descending
						select new
						{
							Id = marketItem.Id,
							Title = marketItem.Title,
							Type = marketItem.Type,
							Buyer = winningBid.User.UserName,
							BuyersRating = buyerfeedback != null ? buyerfeedback.Rating.ToString() : string.Empty,
							Seller = marketItem.User.UserName,
							SellersRating = sellerfeedback != null ? sellerfeedback.Rating.ToString() : string.Empty,
							Status = marketItem.Status,
							CloseDate = marketItem.CloseDate,
							Created = marketItem.Created
						};

					return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
				}
			}
			catch (Exception)
			{
				return model.GetEmptyDataTableResult();
			}
		}
	}
}