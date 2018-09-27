using Cryptopia.Admin.Common.AdminCurrency;
using Cryptopia.Common.DataContext;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Core.Overview
{
	public class OverviewReader : IOverviewReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetDeposits(DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Deposit
					.AsNoTracking()
					.Select(x => new
					{
						Id = x.Id,
						UserName = x.User.UserName,
						Amount = x.Amount,
						Status = x.Status,
						Type = x.Type.ToString(),
						TxId = x.Txid,
						Conf = x.Confirmations,
						Timestamp = x.TimeStamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetTransfers(DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Transfer
					.AsNoTracking()
					.Select(x => new
					{
						Id = x.Id,
						Sender = x.User.UserName,
						Receiver = x.ToUser.UserName,
						Amount = x.Amount,
						Type = x.TransferType,
						Timestamp = x.Timestamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetWithdrawals(DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Withdraw
					.AsNoTracking()
					.Select(x => new
					{
						Id = x.Id,
						UserName = x.User.UserName,
						Amount = x.Amount,
						Status = x.Status,
						Confirmed = x.Confirmed,
						TxId = x.Txid,
						Address = x.Address,
						Conf = x.Confirmations,
						Timestamp = x.TimeStamp,
						Init = x.IsApi ? "API" : "UI"
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetAddresses(DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Address
					.AsNoTracking()
					.Select(x => new
					{
						Id = x.Id,
						UserName = x.User.UserName,
						x.AddressHash
					}).Distinct();

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetLogons(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.UserLogons
					.AsNoTracking()
					.Select(x => new
					{
						UserId = x.User.Id,
						UserName = x.User.UserName,
						IPAddress = x.IPAddress,
						Timestamp = x.Timestamp
					}).Distinct();

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}
	}
}
