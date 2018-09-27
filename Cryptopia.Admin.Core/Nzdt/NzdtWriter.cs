using Cryptopia.Admin.Common.Nzdt;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.Results;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Core.Nzdt
{
	public class NzdtWriter : INzdtWriter
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }
		public IDataContextFactory HubDataContextFactory { get; set; }

		public async Task<IWriterResult> ReprocessNotVerifiedTransaction(string adminUserId, UpdateNzdtTransactionModel model)
		{
			int transactionId;
			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var transaction = await context.NzdtTransaction.Include(x => x.User).FirstOrDefaultNoLockAsync(x => x.Id == model.TransactionId).ConfigureAwait(false);

				if (transaction == null)
				{
					return new WriterResult(false, "Transaction not Found");
				}
				else if (transaction.User == null)
				{
					return new WriterResult(false, "User not Found");

				}
				else if (transaction.TransactionStatus != Enums.NzdtTransactionStatus.ErrorUserNotVerified)
				{
					return new WriterResult(false, "Transaction not in ErrorUserNotVerified Status");
				}
				else if (!(transaction.User.VerificationLevel == Enums.VerificationLevel.Level2 || transaction.User.VerificationLevel == Enums.VerificationLevel.Level3))
				{
					return new WriterResult(false, "User Verification level is not Level2 or Level3");
				}

				transactionId = transaction.Id;

				transaction.TransactionStatus = Enums.NzdtTransactionStatus.ReadyForProcessing;

				await context.SaveChangesAsync().ConfigureAwait(false);
			}

			using (var context = HubDataContextFactory.CreateContext())
			{
				var logMessage = $"[NZDT Import] Reprocessed NotVerified Transaction with Id {transactionId}";

				context.LogActivity(adminUserId, logMessage);
				await context.SaveChangesAsync().ConfigureAwait(false);
			}

			return new WriterResult(true, "Transaction Reprocessed");
		}

		public async Task<IWriterResult> AddUserToTransaction(string adminUserId, UpdateNzdtTransactionModel model)
		{
			int transactionId;
			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var transaction = await context.NzdtTransaction.FirstOrDefaultNoLockAsync(x => x.Id == model.TransactionId).ConfigureAwait(false);

				if (transaction == null)
				{
					return new WriterResult(false, "Transaction not Found");
				}
				else if (transaction.TransactionStatus != Enums.NzdtTransactionStatus.ErrorUserNotFound)
				{
					return new WriterResult(false, "Transaction not in ErrorUserNotFound Status");
				}

				var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.UserName == model.UserName).ConfigureAwait(false);

				if (user == null)
				{
					return new WriterResult(false, "User not Found");
				}
				else if (user.VerificationLevel != Enums.VerificationLevel.Level2 && user.VerificationLevel != Enums.VerificationLevel.Level3)
				{
					return new WriterResult(false, "User Verification level is not Level2 or Level3");
				}

				transactionId = transaction.Id;

				transaction.User = user;
				transaction.TransactionStatus = Enums.NzdtTransactionStatus.ReadyForProcessing;

				await context.SaveChangesAsync().ConfigureAwait(false);
			}

			using (var context = HubDataContextFactory.CreateContext())
			{
				var logMessage = $"[NZDT Import] Added User To Transaction with Id {transactionId}";

				context.LogActivity(adminUserId, logMessage);
				await context.SaveChangesAsync().ConfigureAwait(false);
			}

			return new WriterResult(true, "User Added to Transaction");
		}

	}
}
