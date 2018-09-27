using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;
using System.Collections.Generic;
using Cryptopia.Common.Deposit;

namespace Cryptopia.Common.Currency
{
	public interface ICurrencyReader
	{
		Task<List<CurrencyModel>> GetCurrencies();
		Task<CurrencyModel> GetCurrency(int currencyId);
		Task<CurrencyModel> GetCurrency(string symbol);
		Task<List<BaseCurrencyModel>> GetBaseCurrencies();

		Task<UpdateCurrencyModel> GetUpdateCurrency(int currencyId);
		Task<UpdateCurrencyInfoModel> GetCurrencyInfo(int currencyId);
		Task<CurrencySummaryModel> GetCurrencySummary(int currencyId);
		Task<DataTablesResponse> GetCurrencyDataTable(DataTablesModel model);
		Task<DataTablesResponse> GetCurrencyInfo(DataTablesModel model);
		Task<CurrencyPeerInfoModel> GetPeerInfo(int currencyId);
	}
}