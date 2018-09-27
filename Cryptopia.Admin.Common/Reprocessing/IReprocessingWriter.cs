using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Admin.Common.Reprocessing
{
    public interface IReprocessingWriter
    {
        Task<IWriterResult> UpdateWithrawalTransactionId(string adminUserId, UpdateWithdrawalTxModel model);
        Task<IWriterResult> ApproveWithdrawalReprocessing(string adminUserId, ReprocessingApprovalsModel model);
    }
}
