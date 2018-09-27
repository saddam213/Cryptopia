using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Enums;

namespace Cryptopia.Common.Transfer
{
	public interface ITransferReader
	{
		Task<DataTablesResponse> AdminGetTransfers(string userId, DataTablesModel model);
		Task<TransferModel> GetTransfer(string userId, int id);
		Task<DataTablesResponse> GetTransfers(string userId, DataTablesModel model, TransferType[] types);
	}
}