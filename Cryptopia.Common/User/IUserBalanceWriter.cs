using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.User
{
	public interface IUserBalanceWriter
	{
		Task<IWriterResult> SetFavorite(string userId, int currencyId);
		Task<IWriterResult> DustBalance(string userId, int currencyId);
	}
}
