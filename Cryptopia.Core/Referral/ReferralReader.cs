using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Referral;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Enums;

namespace Cryptopia.Core.Referral
{
	public class ReferralReader : IReferralReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<ReferralInfoModel> GetActiveReferral(string userId)
		{
			var currentUser = new Guid(userId);
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.ReferralInfo
					.AsNoTracking()
					.Where(x => x.UserId == currentUser && x.Status == Enums.ReferralStatus.Active)
					.OrderByDescending(x => x.Id)
					.Select(x => new ReferralInfoModel
					{
						Id = x.Id,
						RoundId = x.RoundId,
						RefCount = x.RefCount,
						Referrer = x.User.Referrer,
						TradeFeeAmount = x.TradeFeeAmount,
						Status = x.Status,
						TransferId = x.TransferId,
						LastUpdate = x.LastUpdate
					});

				var referral = await query.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (referral == null)
					referral = new ReferralInfoModel();

				referral.ActivityBonus = Constant.REFERRAL_ACTIVEBONUS;
				referral.TradePercent = Constant.REFERRAL_TRADEPERCENT;
				return referral;
			}
		}

		public async Task<DataTablesResponse> GetHistory(string userId, DataTablesModel model)
		{
			var currentUser = new Guid(userId);
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.ReferralInfo
					.AsNoTracking()
					.Where(x => x.UserId == currentUser)
					.Select(x => new
					{
						x.Id,
						x.RoundId,
						x.RefCount,
						x.TradeFeeAmount,
						x.Status,
						x.TransferId,
						x.LastUpdate
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> AdminGetHistory(DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.ReferralInfo
					.AsNoTracking()
					.Select(x => new
					{
						x.Id,
						x.RoundId,
						x.User.UserName,
						x.RefCount,
						x.TradeFeeAmount,
						x.Status,
						x.TransferId,
						x.LastUpdate
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}
	}
}