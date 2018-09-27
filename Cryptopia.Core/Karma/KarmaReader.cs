using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Karma;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptopia.Core.Karma
{
	public class KarmaReader : IKarmaReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<UserKarmaModel> GetUserKarma(string userId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				try
				{
					var karmaCount = await context.UserKarma
						.AsNoTracking()
						.Where(k => k.UserId == userId)
						.CountNoLockAsync().ConfigureAwait(false);

					var spentKarma = (await context.UserKarmaHistory
						.AsNoTracking()
						.Where(k => k.UserId == userId)
						.SumAsync(k => (int?) k.Amount).ConfigureAwait(false)) ?? 0;

					return new UserKarmaModel
					{
						Total = karmaCount,
						Available = karmaCount - spentKarma
					};
				}
				catch (Exception)
				{
					return new UserKarmaModel();
				}
			}
		}

		public async Task<DataTablesResponse> GetKarmaHistory(string userId, DataTablesModel model)
		{
			try
			{
				using (var context = DataContextFactory.CreateReadOnlyContext())
				{
					var query = context.UserKarmaHistory
						.AsNoTracking()
						.Where(t => t.UserId == userId)
						.Select(transfer => new
						{
							Id = transfer.Id,
							Timestamp = transfer.Timestamp,
							Amount = transfer.Amount,
							Transaction = transfer.TxId
						});
					return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
				}
			}
			catch (Exception)
			{
				return model.GetEmptyDataTableResult();
			}
		}

		public async Task<DataTablesResponse> GetKarmaReceived(string userId, DataTablesModel model)
		{
			try
			{
				using (var context = DataContextFactory.CreateReadOnlyContext())
				{
					var query = context.UserKarma
						.AsNoTracking()
						.Where(t => t.UserId == userId)
						.Select(transfer => new
						{
							Timestamp = transfer.Timestamp,
							From = transfer.Sender.UserName,
							Type = transfer.KarmaType
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