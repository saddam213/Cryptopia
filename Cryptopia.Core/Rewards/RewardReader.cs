using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Rewards;
using Cryptopia.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;

namespace Cryptopia.Core.Rewards
{
	public class RewardReader : IRewardReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetBalances(string userId, DataTablesModel model)
		{
			var now = DateTime.UtcNow;
			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var query = from currency in context.Currency.Where(p => p.RewardsExpires > now)
										from balance in context.Balance.Where(b => b.CurrencyId == currency.Id && b.UserId == Constant.SYSTEM_USER_REWARD).DefaultIfEmpty()
										select new
										{
											Currency = currency.Symbol,
											Remaining = (decimal?)balance.Total ?? 0,
											CurrencyId = currency.Id
										};

				return await query.GetDataTableResultNoLockAsync(model, true).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetHistory(string userId, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var query = context.Reward
					.AsNoTracking()
					.OrderByDescending(p => p.Id)
					.Select(reward => new
					{
						Id = reward.Id,
						Type = reward.RewardType,
						User = reward.User.UserName,
						Percent = reward.Percent,
						Currency = reward.Currency.Symbol,
						Prize = reward.Amount,
						Time = reward.TimeStamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetStatistics(string userId, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var day = DateTime.UtcNow.AddHours(-24);
				var week = DateTime.UtcNow.AddHours(-(24 * 7));
				var query = context.Reward
					.AsNoTracking()
					.GroupBy(x => x.RewardType)
					.Select(types => new
					{
						Type = types.Key,
						Day = types.Where(x => x.TimeStamp > day)
											.GroupBy(x => x.User.UserName)
											.OrderByDescending(x => x.Count())
											.Select(x => string.Concat(x.Key, "(", x.Count(), ")"))
											.FirstOrDefault() ?? Resources.General.NotAwailable,
						Week = types.Where(x => x.TimeStamp > week)
											.GroupBy(x => x.User.UserName)
											.OrderByDescending(x => x.Count())
											.Select(x => string.Concat(x.Key, "(", x.Count(), ")"))
											.FirstOrDefault() ?? Resources.General.NotAwailable,
						All = types.GroupBy(x => x.User.UserName)
											.OrderByDescending(x => x.Count())
											.Select(x => string.Concat(x.Key, "(", x.Count(), ")"))
											.FirstOrDefault() ?? Resources.General.NotAwailable
					});

				return await query.GetDataTableResultNoLockAsync(model, true).ConfigureAwait(false);
			}
		}
	}
}
