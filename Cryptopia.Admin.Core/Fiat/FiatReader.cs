using Cryptopia.Admin.Common.Fiat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.DataContext;

namespace Cryptopia.Admin.Core.Fiat
{
	public class FiatReader : IFiatReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetDeposits(DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Deposit
					.AsNoTracking()
					.Where(x => x.CurrencyId == Cryptopia.Enums.Constant.NZDT_ID)
					.Select(x => new
					{
						Id = x.Id,
						UserName = x.User.UserName,
						Amount = x.Amount,
						Status = x.Status.ToString(),
						Timestamp = x.TimeStamp,
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
					.Where(x => x.CurrencyId == Cryptopia.Enums.Constant.NZDT_ID)
					.Select(x => new
					{
						Id = x.Id,
						UserName = x.User.UserName,
						Amount = x.Amount,
						Status = x.Status.ToString(),
						Timestamp = x.TimeStamp,
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}
	}
}
