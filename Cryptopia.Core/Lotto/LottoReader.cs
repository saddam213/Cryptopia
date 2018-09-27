using Cryptopia.Common.Lotto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.Extensions;

namespace Cryptopia.Core.Lotto
{
	public class LottoReader : ILottoReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<LottoItemModel> GetLottoItem(int id)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				return await context.LottoItem
					.AsNoTracking()
					.Where(x => x.Id == id)
					.Select(lottoItem => new LottoItemModel
					{
						CurrencyId = lottoItem.CurrencyId,
						CurrentDrawId = lottoItem.CurrentDrawId,
						Description = lottoItem.Description,
						Fee = lottoItem.Fee,
						Hours = lottoItem.Hours,
						LottoItemId = lottoItem.Id,
						LottoType = lottoItem.LottoType,
						Name = lottoItem.Name,
						NextDraw = lottoItem.NextDraw,
						Prizes = lottoItem.Prizes,
						Rate = lottoItem.Rate,
						Status = lottoItem.Status,
						Symbol = lottoItem.Currency.Symbol,
						TicketsInDraw = lottoItem.Tickets.Count
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
			}
		}

		public async Task<List<LottoItemModel>> GetLottoItems(string userId)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var currentUser = !string.IsNullOrEmpty(userId) ? new Guid(userId) : Guid.Empty;
				var lottoItems = await context.LottoItem
					.AsNoTracking()
					.Where(x => x.Status != Enums.LottoItemStatus.Disabled)
					.Select(lottoItem => new LottoItemModel
					{
						CurrencyId = lottoItem.CurrencyId,
						CurrentDrawId = lottoItem.CurrentDrawId,
						Description = lottoItem.Description,
						Fee = lottoItem.Fee,
						Hours = lottoItem.Hours,
						LottoItemId = lottoItem.Id,
						LottoType = lottoItem.LottoType,
						Name = lottoItem.Name,
						NextDraw = lottoItem.NextDraw,
						Prizes = lottoItem.Prizes,
						Rate = lottoItem.Rate,
						Status = lottoItem.Status,
						Symbol = lottoItem.Currency.Symbol,
						TicketsInDraw = lottoItem.Tickets.Count(x => x.DrawId == lottoItem.CurrentDrawId),
						UserTicketsInDraw = lottoItem.Tickets.Count(x => x.DrawId == lottoItem.CurrentDrawId && x.UserId == currentUser)
					}).ToListNoLockAsync().ConfigureAwait(false);

				lottoItems.ForEach(x => x.PrizeInfo = GetPrizeInfo(x));
				return lottoItems;
			}
		}

		public async Task<DataTablesResponse> GetLottoItems(DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.LottoItem
					.AsNoTracking()
					.Select(lottoItem => new
					{
						Id = lottoItem.Id,
						Name = lottoItem.Name,
						Symbol = lottoItem.Currency.Symbol,
						Type = lottoItem.LottoType,
						Status = lottoItem.Status,
						NextDraw = lottoItem.NextDraw,
						CurrentDraw = lottoItem.CurrentDrawId,
						Tickets = lottoItem.Tickets.Count
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetHistory(DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.LottoHistory
					.AsNoTracking()
					.Select(history => new
					{
						Name = history.LottoItem.Name,
						Draw = history.LottoDrawId,
						Symbol = history.LottoItem.Currency.Symbol,
						TotalPrize = history.TotalAmount,
						Ticket = history.LottoTicketId,
						User = history.Ticket.User.UserName,
						PrizePosition = history.Position,
						PrizePercent = history.Percent,
						Prize = history.Amount,
						Timestamp = history.Timestamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetUserHistory(string userId, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var currentUser = new Guid(userId);
				var query = context.LottoHistory
					.AsNoTracking()
					.Where(x => x.UserId == currentUser)
					.Select(history => new
					{
						Name = history.LottoItem.Name,
						Draw = history.LottoDrawId,
						Symbol = history.LottoItem.Currency.Symbol,
						TotalPrize = history.TotalAmount,
						Ticket = history.LottoTicketId,
						PrizePosition = history.Position,
						PrizePercent = history.Percent,
						Prize = history.Amount,
						Timestamp = history.Timestamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetUserTickets(string userId, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var currentUser = new Guid(userId);
				var query = context.LottoTicket
					.AsNoTracking()
					.Where(x => x.UserId == currentUser && !x.IsArchived)
					.Select(ticket => new
					{
						Id = ticket.Id,
						Name = ticket.LottoItem.Name,
						DrawId = ticket.DrawId,
						Symbol = ticket.LottoItem.Currency.Symbol,
						Price = ticket.LottoItem.Rate,
						NextDraw = ticket.LottoItem.NextDraw,
						Timestamp = ticket.Timestamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}


		private List<LottoPrizeInfoModel> GetPrizeInfo(LottoItemModel lottoItem)
		{
			var prizeInfo = new List<LottoPrizeInfoModel>();
			var totalAmount = lottoItem.Rate*lottoItem.TicketsInDraw;
			var siteFee = (totalAmount/100m)*lottoItem.Fee;
			var prizePool = totalAmount - siteFee;
			var prizePoolFraction = prizePool/100m;
			var prizeWeights = LottoHelpers.GetPrizeWeights(lottoItem.Prizes);

			for (int i = 0; i < prizeWeights.Count; i++)
			{
				prizeInfo.Add(new LottoPrizeInfoModel
				{
					Percentage = prizeWeights[i],
					Position = (i + 1),
					Prize = Math.Round(prizePoolFraction*prizeWeights[i], 8)
				});
			}
			return prizeInfo;
		}
	}
}