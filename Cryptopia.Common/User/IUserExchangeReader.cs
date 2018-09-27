using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Common.User
{
	public interface IUserExchangeReader
	{
		Task<DataTablesResponse> GetTradeDataTable(string userId, DataTablesModel model);
		Task<DataTablesResponse> GetTradeHistoryDataTable(string userId, DataTablesModel model);
	}
}