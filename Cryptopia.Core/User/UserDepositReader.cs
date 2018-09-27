using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Enums;

namespace Cryptopia.Core.User
{
	public class UserDepositReader : IUserDepositReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetDataTable(string userId, DataTablesModel model)
		{
			try
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var currentUserId = new Guid(userId);
					var query = context.Deposit
						.AsNoTracking()
						.Where(t => t.UserId == currentUserId && t.Type != DepositType.Mining && t.Status != DepositStatus.Invalid)
						.Select(transfer => new UserDepositDataTableModel
						{
							Id = transfer.Id,
							Symbol = transfer.Currency.Symbol,
							Amount = transfer.Amount,
							Confirmations = string.Concat(transfer.Confirmations > transfer.Currency.MinConfirmations
								? transfer.Currency.MinConfirmations
								: transfer.Confirmations, "/", transfer.Currency.MinConfirmations),
							Status = transfer.Status,
							TransactionId = transfer.Txid,
							TimeStamp = transfer.TimeStamp,
							Type = transfer.Type
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