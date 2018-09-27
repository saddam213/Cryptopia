using Cryptopia.Infrastructure.Common.DataTables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptopia.Common.Lotto
{
	public interface ILottoReader
	{
		Task<LottoItemModel> GetLottoItem(int id);
		Task<List<LottoItemModel>> GetLottoItems(string userId);
		Task<DataTablesResponse> GetLottoItems(DataTablesModel model);

		Task<DataTablesResponse> GetHistory(DataTablesModel model);
		Task<DataTablesResponse> GetUserTickets(string userId, DataTablesModel model);
		Task<DataTablesResponse> GetUserHistory(string userId, DataTablesModel model);
	}
}
