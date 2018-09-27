using Cryptopia.Infrastructure.Common.DataTables;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.Nzdt
{
	public interface INzdtReader
	{
		Task<UpdateNzdtTransactionModel> GetUpdateTransaction(int transactionId);

		Task<DataTablesResponse> GetAllTransactions(DataTablesModel model);
		Task<DataTablesResponse> GetReadyTransactions(DataTablesModel model);
		Task<DataTablesResponse> GetProcessedTransactions(DataTablesModel model);
		Task<DataTablesResponse> GetErroredTransactions(DataTablesModel model);

	}
}
