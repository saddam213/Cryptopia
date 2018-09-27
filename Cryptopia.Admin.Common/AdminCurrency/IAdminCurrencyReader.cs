using Cryptopia.Common.Currency;
using Cryptopia.Infrastructure.Common.DataTables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.AdminCurrency
{
	public interface IAdminCurrencyReader
	{
		Task<AdminCurrencyInfoModel> GetCurrency(string symbol);
		Task<AdminCurrencyInfoModel> GetCurrency(int currencyId);

		Task<DataTablesResponse> GetCurrencies(DataTablesModel model);
		Task<UpdateCurrencyModel> GetUpdateCurrency(int currencyId);
		Task<UpdateListingStatusModel> GetUpdateListingStatusModel(int currencyId);
		Task<DataTablesResponse> GetDeposits(int currencyId, DataTablesModel model);
		Task<DataTablesResponse> GetWithdrawals(int currencyId, DataTablesModel model);
		Task<DataTablesResponse> GetTransfers(int currencyId, DataTablesModel model);
		Task<DataTablesResponse> GetAddresses(int currencyId, DataTablesModel model);
		Task<List<CurrencyModel>> GetCurrencies();
	}
}
