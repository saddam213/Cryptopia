using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.Karma;
using Cryptopia.Entity;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.Karma
{
	public class KarmaWriter : IKarmaWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IWriterResult> CreateKarma(string userId, CreateKarmaModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				try
				{
					var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
					if (user == null)
						return new WriterResult(false);

					var karma = new UserKarma
					{
						UserId = model.UserId,
						KarmaType = model.Type,
						SenderId = userId,
						Discriminator = model.Discriminator
					};

					context.UserKarma.Add(karma);
					await AuditKarma(context, user).ConfigureAwait(false);
					await context.SaveChangesAsync().ConfigureAwait(false);
					return new WriterResult(true);
				}
				catch (DbUpdateException)
				{
					return new WriterResult(false);
				}
			}
		}

		public async Task<IWriterResult> SpendKarma(string userId, SpendKarmaModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				using (var contextTransaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Snapshot))
				{
					try
					{
						if (model.Amount < 1)
							return new WriterResult(false, "Invalid karma amount");

						var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
						if (user == null)
							return new WriterResult(false);

						var spendableKarma = await AuditKarma(context, user);
						if (model.Amount > spendableKarma)
							return new WriterResult(false, "Not enough unspent karma");

						var karmaHistory = new UserKarmaHistory
						{
							UserId = userId,
							Amount = model.Amount,
							TxId = model.TxId,
						};

						context.UserKarmaHistory.Add(karmaHistory);
						await context.SaveChangesAsync().ConfigureAwait(false);

						spendableKarma = await AuditKarma(context, user).ConfigureAwait(false);
						if (spendableKarma < 0)
						{
							contextTransaction.Rollback();
							return new WriterResult(false);
						}

						contextTransaction.Commit();
						return new WriterResult(true);
					}
					catch (Exception)
					{
						contextTransaction.Rollback();
						throw;
					}
				}
			}
		}

		private async Task<int> AuditKarma(IDataContext context, ApplicationUser user)
		{
			var userId = user.Id;
			var karmaCount = await context.UserKarma
				.Where(k => k.UserId == userId)
				.CountNoLockAsync().ConfigureAwait(false);

			var spentKarma = (await context.UserKarmaHistory
				.Where(k => k.UserId == userId)
				.SumAsync(k => (int?) k.Amount).ConfigureAwait(false)) ?? 0;

			if (user.KarmaTotal != karmaCount)
			{
				user.KarmaTotal = karmaCount;
				await context.SaveChangesAsync().ConfigureAwait(false);
			}

			var totalKarma = karmaCount - spentKarma;
			return totalKarma >= 0 ? totalKarma : -1;
		}
	}
}