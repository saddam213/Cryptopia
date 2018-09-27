using Cryptopia.Infrastructure.Common.DataTables;
using System.Threading.Tasks;

namespace Cryptopia.Common.Balance
{
	public interface IBalanceReader
	{
		Task<DataTablesResponse> GetTradeBalances(string userId, DataTablesModel model);
		Task<BalanceTradePairModel> GetTradePairBalance(string userId, int tradePairId);
		Task<BalanceCurrencyModel> GetCurrencyBalance(string userId, int currencyId);
	}
}
