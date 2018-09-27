using Cryptopia.Enums;
using System.Threading.Tasks;

namespace Cryptopia.Common.External
{
	public interface IExternalApiService
	{
		Task<decimal> ConvertDollarToBTC(decimal dollarAmount, ConvertDollarToBTCType dollarType);
	}


}
