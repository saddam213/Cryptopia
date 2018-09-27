using Cryptopia.Common.TradePair;
using Cryptopia.Infrastructure.Common.DataTables;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.AdminCurrency
{
	public interface IAdminTradePairReader
	{
		Task<DataTablesResponse> GetTradePairs(DataTablesModel model);
		Task<UpdateTradePairModel> GetTradePair(int id);
	}
}
