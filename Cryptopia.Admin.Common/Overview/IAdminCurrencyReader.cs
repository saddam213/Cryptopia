using Cryptopia.Common.Currency;
using Cryptopia.Infrastructure.Common.DataTables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.AdminCurrency
{
	public interface IOverviewReader
	{
		Task<DataTablesResponse> GetDeposits(DataTablesModel model);
		Task<DataTablesResponse> GetWithdrawals(DataTablesModel model);
		Task<DataTablesResponse> GetTransfers(DataTablesModel model);
		Task<DataTablesResponse> GetAddresses(DataTablesModel model);
		Task<DataTablesResponse> GetLogons(DataTablesModel model);
	}
}
