using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Enums;

namespace Cryptopia.Core.User
{
	public class UserMineshaftReader : IUserMineshaftReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetHistory(string userId, DataTablesModel model)
		{
			try
			{
				var currentUserId = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var query = context.Deposit
						.AsNoTracking()
						.Where(t => t.UserId == currentUserId && t.Type == DepositType.Mining && t.Status != DepositStatus.Invalid)
						.Select(transfer => new
						{
							Id = transfer.Id,
							TimeStamp = transfer.TimeStamp,
							Symbol = transfer.Currency.Symbol,
							Type = "Normal",
							Amount = transfer.Amount,
							Confirmations = string.Concat(transfer.Confirmations > transfer.Currency.MinConfirmations
								? transfer.Currency.MinConfirmations
								: transfer.Confirmations, "/", transfer.Currency.MinConfirmations),
							Status = transfer.Status,
							TransactionId = transfer.Txid,
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