using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Admin.Common.Reprocessing
{
    public interface IReprocessingReader
    {
        Task<UpdateWithdrawalTxModel> GetWithdrawalToUpdate(int id);
        Task<DataTablesResponse> GetIncompleteWithdrawals(DataTablesModel model);
        Task<DataTablesResponse> GetWalletTransactions(WalletTxRequestModel model, DataTablesModel tableModel);
        Task<DataTablesResponse> GetPendingApprovals(DataTablesModel model);
        Task<ReprocessingApprovalsModel> GetApproval(int id);
    }
}
