using Cryptopia.Admin.Common.Nzdt;
using Cryptopia.Common.DataContext;
using Cryptopia.Entity;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.Results;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Admin.Core.Nzdt
{
	public class NzdtReader : INzdtReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }


		public async Task<UpdateNzdtTransactionModel> GetUpdateTransaction(int transactionId)
		{

			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var transaction = await context.NzdtTransaction
					.Where(x => x.Id == transactionId)
					.Where(x => x.DepositId == null || x.ProcessedOn == null)
					.Where(x => x.TransactionStatus == NzdtTransactionStatus.ErrorUserNotFound || x.TransactionStatus == NzdtTransactionStatus.ErrorUserNotVerified)
					.Select(x => new UpdateNzdtTransactionModel
					{
						TransactionId = x.Id,
						Status = x.TransactionStatus,

						UniqueId = x.UniqueId,
						Amount = x.Amount,
						Date = x.Date,
						Memo = x.Memo,
						VerificationLevel = x.UserId.HasValue ? x.User.VerificationLevel.ToString() : "",
						UserName = x.UserId.HasValue ? x.User.UserName : "",
						CreatedOn = x.CreatedOn
					})
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);

				return transaction;
			}

		}


		public async Task<DataTablesResponse> GetAllTransactions(DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.NzdtTransaction
					.AsNoTracking()
					.Select(x => new
					{
						Id = x.Id,
						UniqueId = x.UniqueId,
						Amount = x.Amount,
						Memo = x.Memo,
						Date = x.Date,
						DepositId = x.DepositId.HasValue ? x.DepositId.ToString() : "No Deposit Found",
						UserName = x.UserId.HasValue ? x.User.UserName : "No User Found",
						Status = x.TransactionStatus.ToString(),
						CreatedOn = x.CreatedOn
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetReadyTransactions(DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.NzdtTransaction
					.AsNoTracking()
					.Where(x => x.TransactionStatus == NzdtTransactionStatus.ReadyForProcessing)
					.Select(x => new
					{
						Id = x.Id,
						UniqueId = x.UniqueId,
						Amount = x.Amount,
						UserName = x.User.UserName,
						CreatedOn = x.CreatedOn
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetProcessedTransactions(DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.NzdtTransaction
					.AsNoTracking()
					.Where(x => x.TransactionStatus == NzdtTransactionStatus.Processed)
					.Select(x => new
					{
						Id = x.Id,
						UniqueId = x.UniqueId,
						Amount = x.Amount,
						DepositId = x.DepositId,
						UserName = x.User.UserName,
						ProcessedOn = x.ProcessedOn
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetErroredTransactions(DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.NzdtTransaction
					.AsNoTracking()
					.Where(x => x.TransactionStatus == NzdtTransactionStatus.ErrorUserNotFound || x.TransactionStatus == NzdtTransactionStatus.ErrorUserNotVerified)
					.Select(x => new
					{
						Id = x.Id,
						UniqueId = x.UniqueId,
						Amount = x.Amount,
						Memo = x.Memo,
						Date = x.Date,
						UserName = x.UserId.HasValue ? x.User.UserName : "No User Found",
						VerificationLevel = x.UserId.HasValue ? x.User.VerificationLevel.ToString() : "No User Found",
						Status = x.TransactionStatus.ToString(),
						CreatedOn = x.CreatedOn
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}
	}
}
