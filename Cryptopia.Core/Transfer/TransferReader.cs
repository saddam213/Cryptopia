using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Transfer;
using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using Cryptopia.Common.DataContext;

namespace Cryptopia.Core.Transfer
{
	public class TransferReader : ITransferReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetTransfers(string userId, DataTablesModel model, TransferType[] types)
		{
			var currentUser = new Guid(userId);
			var transferTypes = new List<TransferType>(types);
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Transfer
					.AsNoTracking()
					.Where(x => (x.UserId == currentUser || x.ToUserId == currentUser) && transferTypes.Contains(x.TransferType))
					.Select(transfer => new
					{
						Id = transfer.Id,
						Currency = transfer.Currency.Symbol,
						Amount = transfer.Amount,
						Sender = transfer.User.UserName,
						Receiver = transfer.ToUser.UserName,
						Type = transfer.TransferType,
						Timestamp = transfer.Timestamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}


		public async Task<DataTablesResponse> AdminGetTransfers(string userId, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Transfer
					.AsNoTracking()
					.Where(x => x.TransferType != TransferType.Reward && x.TransferType != TransferType.Tip)
					.Select(transfer => new
					{
						Id = transfer.Id,
						Currency = transfer.Currency.Symbol,
						Amount = transfer.Amount,
						Sender = transfer.User.UserName,
						Receiver = transfer.ToUser.UserName,
						Type = transfer.TransferType,
						Timestamp = transfer.Timestamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<TransferModel> GetTransfer(string userId, int id)
		{
			var currentUser = new Guid(userId);
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				return await context.Transfer
					.AsNoTracking()
					.Where(x => x.Id == id && x.UserId == currentUser)
					.Select(transfer => new TransferModel
					{
						Id = transfer.Id,
						Symbol = transfer.Currency.Symbol,
						Amount = transfer.Amount,
						Name = transfer.Currency.Name,
						Receiver = transfer.ToUser.UserName,
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
			}
		}
	}
}