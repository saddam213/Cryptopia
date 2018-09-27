using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.Currency
{
	public interface ICurrencyWriter
	{
		Task<IWriterResult> UpdateCurrency(UpdateCurrencyModel model);
		Task<IWriterResult> UpdateCurrencyInfo(UpdateCurrencyInfoModel model);
	}
}