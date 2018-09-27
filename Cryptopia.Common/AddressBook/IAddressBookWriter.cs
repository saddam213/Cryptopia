using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.Address
{
	public interface IAddressBookWriter
	{
		Task<IWriterResult> CreateAddressBook(string userId, AddressBookModel model);
		Task<IWriterResult> DeleteAddressBook(string userId, int addressbookId);
	}
}
