using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Core.User
{
	public class UserWithdrawReader : IUserWithdrawReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetDataTable(string userId, DataTablesModel model)
		{
			try
			{
				var currentUserId = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var query = context.Withdraw
						.AsNoTracking()
						.Where(t => t.UserId == currentUserId)
						.Select(transfer => new UserWithdrawDataTableModel
						{
							Id = transfer.Id,
							Symbol = transfer.Currency.Symbol,
							Amount = transfer.Amount,
							Fee = transfer.Fee,
							Status = transfer.Status,
							TransactionId = transfer.Txid,
							Address = transfer.Address,
							TimeStamp = transfer.TimeStamp
						});
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