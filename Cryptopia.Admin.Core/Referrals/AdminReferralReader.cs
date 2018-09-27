using Cryptopia.Admin.Common.Referral;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Core.Referrals
{
	public class AdminReferralReader : IAdminReferralReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetHistory(DataTablesModel model)
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
