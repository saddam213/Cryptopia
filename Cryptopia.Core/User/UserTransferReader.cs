using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Core.User
{
	public class UserTransferReader : IUserTransferReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetDataTable(string userId, DataTablesModel model)
		{
			try
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var currentUserId = new Guid(userId);
					var query = context.Transfer
						.AsNoTracking()
						.Where(t => t.UserId == currentUserId || t.ToUserId == currentUserId)
						.Select(transfer => new UserTransferDataTableModel
						{
							Id = transfer.Id,
							Symbol = transfer.Currency.Symbol,
							Amount = transfer.Amount,
							Fee = transfer.Fee,
							TransferType = transfer.TransferType,
							Sender = transfer.User.UserName,
							Receiver = transfer.ToUser.UserName,
							TimeStamp = transfer.Timestamp
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