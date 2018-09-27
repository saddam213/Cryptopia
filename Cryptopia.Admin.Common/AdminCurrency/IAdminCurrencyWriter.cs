using Cryptopia.Common.Currency;
using Cryptopia.Infrastructure.Common.Results;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.AdminCurrency
{
	public interface IAdminCurrencyWriter
	{
		Task<IWriterResult> UpdateCurrency(string adminUserId, UpdateCurrencyModel model);
		Task<IWriterResult> BeginDelistingCurrency(string adminUserId, UpdateListingStatusModel model);
	}
}
