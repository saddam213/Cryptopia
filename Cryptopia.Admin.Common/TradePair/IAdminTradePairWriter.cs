using Cryptopia.Common.TradePair;
using Cryptopia.Infrastructure.Common.Results;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.AdminCurrency
{
	public interface IAdminTradePairWriter
	{
		Task<IWriterResult> CreateTradePair(string adminUserId, CreateTradePairModel model);
		Task<IWriterResult> UpdateTradePair(string adminUserId, UpdateTradePairModel model);
	}
}
