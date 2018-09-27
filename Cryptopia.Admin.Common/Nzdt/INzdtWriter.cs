using Cryptopia.Infrastructure.Common.Results;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.Nzdt
{
	public interface INzdtWriter
	{
		Task<IWriterResult> ReprocessNotVerifiedTransaction(string adminUserId, UpdateNzdtTransactionModel model);
		Task<IWriterResult> AddUserToTransaction(string adminUserId, UpdateNzdtTransactionModel model);
	}
}
