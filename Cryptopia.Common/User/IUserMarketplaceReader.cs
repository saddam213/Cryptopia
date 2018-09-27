using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Common.User
{
	public interface IUserMarketplaceReader
	{
		Task<DataTablesResponse> GetMarketItems(string userId, DataTablesModel model);
		Task<DataTablesResponse> GetMarketHistory(string userId, DataTablesModel model);
	}
}