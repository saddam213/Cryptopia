using Cryptopia.Common.TermDeposits;
using System;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using System.Data.Entity;
using Cryptopia.Enums;
using System.Transactions;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.TermDeposits
{
	public class TermDepositWriter : ITermDepositWriter
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<IWriterResult> CreateTermDeposit(string userId, CreateTermDepositModel model)
		{
			using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }, TransactionScopeAsyncFlowOption.Enabled))
			{
				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var user = await context.Users.FirstOrDefaultAsync(x => x.Id == currentUser).ConfigureAwait(false);
					if (user == null)
						return new WriterResult<int>(false, "Unauthorized");

					var termDepositItem = await context.TermDepositItem.FirstOrDefaultAsync(x => x.Id == model.Id).ConfigureAwait(false);
					if (termDepositItem == null)
						return new WriterResult<int>(false, "Invalid term deposit");

					var totalTermDeposit = termDepositItem.MinInvestment * model.Count;
					var balance = await context.Balance.FirstOrDefaultAsync(x => x.UserId == user.Id && x.CurrencyId == Constant.DOTCOIN_ID).ConfigureAwait(false);
					if (balance == null || balance.Available < totalTermDeposit)
						return new WriterResult<int>(false, "Insufficient funds for term deposit.");

					var withdrawal = new Entity.Withdraw
					{
						Address = termDepositItem.Address,
						Amount = totalTermDeposit,
						Confirmations = 0,
						CurrencyId = Constant.DOTCOIN_ID,
						Fee = 0,
						IsApi = false,
						Status = WithdrawStatus.Pending,
						TimeStamp = DateTime.UtcNow,
						TwoFactorToken = string.Empty,
						Txid = string.Empty,
						Type = WithdrawType.TermDeposit,
						UserId = user.Id
					};

					context.Withdraw.Add(withdrawal);
					await context.SaveChangesAsync().ConfigureAwait(false);
					await context.AuditUserBalance(user.Id, balance.CurrencyId).ConfigureAwait(false);

					var termBegin = DateTime.UtcNow;
					var termEnd = termBegin.AddDays(30 * termDepositItem.TermLength);
					var nextPayment = termBegin.AddDays(30);
					var termdeposit = new Entity.TermDeposit
					{
						UserId = user.Id,
						WithdrawId = withdrawal.Id,
						TermDepositItemId = termDepositItem.Id,
						Status = TermDepositStatus.Active,
						TermBegin = termBegin,
						NextPayout = nextPayment,
						TermEnd = termEnd,
						Closed = termEnd
					};
					context.TermDeposit.Add(termdeposit);
					await context.SaveChangesAsync().ConfigureAwait(false);
					transactionScope.Complete();
					return new WriterResult(true, $"Successfully created term deposit #{termdeposit.Id}");
				}
			}
		}

		public async Task<IWriterResult> CancelTermDeposit(string userId, int termDepositId)
		{
			var currentUser = new Guid(userId);
			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultAsync(x => x.Id == currentUser).ConfigureAwait(false);
				if (user == null)
					return new WriterResult<int>(false, "Unauthorized");

				var termDeposit = await context.TermDeposit.FirstOrDefaultAsync(x => x.Id == termDepositId && x.UserId == user.Id && x.Status == TermDepositStatus.Active).ConfigureAwait(false);
				if (termDeposit == null)
					return new WriterResult<int>(false, $"Term deposit #{termDeposit.Id} not found or is no longer active.");

				termDeposit.Closed = DateTime.UtcNow;
				termDeposit.Status = TermDepositStatus.PendingCancel;
				var cancelPayment = new Entity.TermDepositPayment
				{
					Amount = termDeposit.Withdraw.Amount,
					TermDepositId = termDeposit.Id,
					Timestamp = DateTime.UtcNow,
					TransactionId = "Pending ...",
					Type = TermDepositPaymentType.Cancel
				};
				context.TermDepositPayment.Add(cancelPayment);
				await context.SaveChangesAsync().ConfigureAwait(false);
				return new WriterResult(true, $"Successfully canceled term deposit #{termDeposit.Id}");
			}
		}

		public async Task<IWriterResult> AdminUpdatePayment(string userId, UpdateTermDepositPaymentModel model)
		{
			var currentUser = new Guid(userId);
			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultAsync(x => x.Id == currentUser).ConfigureAwait(false);
				if (user == null)
					return new WriterResult<int>(false, "Unauthorized");

				var termDepositPayment = await context.TermDepositPayment
					.Include(x => x.TermDeposit)
					.FirstOrDefaultAsync(x => x.Id == model.PaymentId).ConfigureAwait(false);
				if (termDepositPayment == null)
					return new WriterResult<int>(false, $"Term deposit payment #{model.PaymentId} not found.");

				termDepositPayment.TransactionId = model.TransactionId;

				if (termDepositPayment.Type == TermDepositPaymentType.Cancel && termDepositPayment.TermDeposit.Status == TermDepositStatus.PendingCancel)
					termDepositPayment.TermDeposit.Status = TermDepositStatus.Canceled;
			
				await context.SaveChangesAsync().ConfigureAwait(false);
				return new WriterResult(true, $"Successfully updated term deposit #{termDepositPayment.TermDepositId}");
			}
		}
	}
}
