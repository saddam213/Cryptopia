using System.Collections.Generic;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Common.TermDeposits
{
	public interface ITermDepositReader
	{
		Task<DataTablesResponse> GetOpenDeposits(string userId, DataTablesModel model);
		Task<DataTablesResponse> GetPayouts(string userId, DataTablesModel model);
		Task<DataTablesResponse> GetClosedDeposits(string userId, DataTablesModel model);

		Task<TermDepositItemModel> GetTermDepositItem(int id);
		Task<List<TermDepositItemModel>> GetTermDepositItems();
		Task<List<TermDepositSummaryModel>> GetTermDepositSummary();

		Task<UpdateTermDepositPaymentModel> AdminGetPayout(string userId, int id);
		Task<DataTablesResponse> AdminGetDeposits(string userId, DataTablesModel model);
		Task<DataTablesResponse> AdminGetPayouts(string userId, DataTablesModel model);
	}
}
