using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Enums;

namespace Cryptopia.Core.User
{
	public class UserExchangeReader : IUserExchangeReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetTradeDataTable(string userId, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				try
				{
					var currentUserId = new Guid(userId);
					var query = context.Trade
						.AsNoTracking()
						.Where(t => t.UserId == currentUserId && (t.Status == TradeStatus.Partial || t.Status == TradeStatus.Pending))
						.Select(transfer => new UserTradeDataTableModel
						{
							Id = transfer.Id,
							TradePairId = transfer.TradePairId,
							Market = string.Concat(transfer.TradePair.Currency1.Symbol, "/", transfer.TradePair.Currency2.Symbol),
							Type = transfer.Type,
							Rate = transfer.Rate,
							Amount = transfer.Amount,
							Remaining = transfer.Remaining,
							Total = Math.Round(transfer.Amount*transfer.Rate, 8),
							Fee = transfer.Fee,
							TimeStamp = transfer.Timestamp,
						});
					return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
				}
				catch (Exception)
				{
					return model.GetEmptyDataTableResult();
				}
			}
		}

		public async Task<DataTablesResponse> GetTradeHistoryDataTable(string userId, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				try
				{
					var currentUserId = new Guid(userId);
					var query = context.TradeHistory
						.AsNoTracking()
						.Where(t => t.UserId == currentUserId || t.ToUserId == currentUserId)
						.Select(transfer => new UserTradeHistoryDataTableModel
						{
							Id = transfer.Id,
							Market = string.Concat(transfer.TradePair.Currency1.Symbol, "/", transfer.TradePair.Currency2.Symbol),
							Type = transfer.ToUserId == currentUserId ? TradeHistoryType.Sell : TradeHistoryType.Buy,
							Amount = transfer.Amount,
							Rate = transfer.Rate,
							Total = Math.Round(transfer.Amount*transfer.Rate, 8),
							Fee = transfer.Fee,
							TimeStamp = transfer.Timestamp
						});
					return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
				}
				catch (Exception)
				{
					return model.GetEmptyDataTableResult();
				}
			}
		}
	}
}