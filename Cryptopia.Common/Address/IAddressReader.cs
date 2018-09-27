using Cryptopia.Enums;
using System.Threading.Tasks;

namespace Cryptopia.Common.Address
{
	public interface IAddressReader
	{
		Task<AddressModel> GetAddress(string userId, int currencyId);
		Task<DisplayAddressModel> GetDisplayAddress(string userId, string symbol);
	}
}
