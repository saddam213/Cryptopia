using System.Collections.Generic;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Common.Address
{
	public interface IAddressBookReader
	{
		Task<List<AddressBookModel>> GetAddressBook(string userId, int currencyId);
		Task<DataTablesResponse> GetAddressBookDataTable(string userId, DataTablesModel model);
		Task<bool> AddressBookEntryExists(string userId, int currencyId, string address);
	}
}