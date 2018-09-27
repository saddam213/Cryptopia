using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Deposit;
using System;
using Cryptopia.Common.DataContext;
using Cryptopia.Enums;

namespace Cryptopia.Core.Deposit
{
	public class DepositReader : IDepositReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<DataTablesResponse> AdminGetDeposits(string userId, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				context.Database.CommandTimeout = 120;
				var maxDate = DateTime.UtcNow.AddDays(-30);
				var query = context.Deposit
					.AsNoTracking()
					.Where(x => x.TimeStamp > maxDate && x.Type == DepositType.Normal)
					.Select(deposit => new
					{
						Id = deposit.Id,
						UserName = deposit.User.UserName,
						Currency = deposit.Currency.Symbol,
						Amount = deposit.Amount,
						Status = deposit.Status,
						Type = deposit.Type,
						TxId = deposit.Txid,
						Conf = deposit.Confirmations,
						Timestamp = deposit.TimeStamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}
	}
}