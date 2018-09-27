using System;
using System.Threading.Tasks;
using Cryptopia.Admin.Common.Reprocessing;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Infrastructure.Common.DataContext;
using System.Linq;
using Cryptopia.WalletAPI.DataObjects;
using Cryptopia.Admin.Core.AdmintopiaService;
using System.Collections.Generic;
using Cryptopia.Entity;
using Cryptopia.Enums;
using Cryptopia.Admin.Common.Approval;
using Cryptopia.Common.Cache;
using Newtonsoft.Json;

namespace Cryptopia.Admin.Core.Reprocessing
{
    public class ReprocessingReader : IReprocessingReader
    {
        public IDataContextFactory DataContextFactory { get; set; }
        public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

        public ICacheService CacheService { get; set; }

        public int WalletTimeoutMinutes { get; set; } = 1;

        public async Task<UpdateWithdrawalTxModel> GetWithdrawalToUpdate(int id)
        {
            using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
            {
                return await context.Withdraw
                    .AsNoTracking()
                    .Where(x => x.Status == Enums.WithdrawStatus.Processing)
                    .Select(x => new UpdateWithdrawalTxModel
                    {
                        Id = x.Id,
                        TxId = x.Txid,
                        Status = x.Status,
                        Address = x.Address,
                        Amount = x.Amount,
                        RetryCount = x.RetryCount
                    }).FirstOrDefaultNoLockAsync(x => x.Id == id).ConfigureAwait(false);
            }
        }

        public async Task<DataTablesResponse> GetIncompleteWithdrawals(DataTablesModel model)
        {
            var newest = DateTime.Now.AddHours(-2.0);
            List<int> pendingApprovalIds = null;

            using (var approvalContext = DataContextFactory.CreateReadOnlyContext())
            {
                var approvals = await approvalContext.ApprovalQueue
                    .Where(a => a.Type == ApprovalQueueType.WithdrawalReprocessing && a.Status == ApprovalQueueStatus.Pending)
                    .Select(a => a)
                    .ToListNoLockAsync().ConfigureAwait(false);

                pendingApprovalIds = approvals.Select(a =>
                    {
                        int id;
                        bool success = int.TryParse(JsonConvert.DeserializeObject<ReprocessingApprovalDataModel>(a.Data).WithdrawalId, out id);
                        return new { success, id };
                    })
                    .Where(x => x.success)
                    .Select(x => x.id).ToList();
            }

            using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
            {
                var query = context.Withdraw
                    .AsNoTracking()
                    .Where(w => w.Status == Enums.WithdrawStatus.Processing && w.Confirmed <= newest && !pendingApprovalIds.Contains(w.Id))
                    .Select(x => new
                    {
                        Id = x.Id,
                        UserName = x.User.UserName,
                        Currency = x.Currency.Symbol,
                        Amount = x.Amount,
                        Address = x.Address,
                        Confirmed = x.Confirmed,
                        RetryCount = x.RetryCount
                    });

                var result = await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
                return result;
            }
        }

        public async Task<DataTablesResponse> GetWalletTransactions(WalletTxRequestModel model, DataTablesModel tableModel)
        {
            var cacheResult = await CacheService.GetOrSetMemoryAsync(CacheKey.WalletTransactions(model.Currency), TimeSpan.FromMinutes(10), async () =>
            {
                int currencyId = -1;

                using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
                {
                    Entity.Currency selectedCurrency = await context.Currency.FirstOrDefaultNoLockAsync(c => c.Symbol.Equals(model.Currency));

                    if (selectedCurrency != null)
                        currencyId = selectedCurrency.Id;
                }

                if (currencyId == -1)
                    return null;

                List<WalletTransaction> transactions = new List<WalletTransaction>();
                using (var service = new AdmintopiaServiceClient())
                    transactions = await service.GetWalletTransactionsSinceAsync(AdmintopiaService.TransactionDataType.Withdraw, currencyId, WalletTimeoutMinutes, model.BlockLength);

                return transactions.Select(x => new
                {
                    Type = x.Type,
                    Amount = x.Amount,
                    Txid = x.Txid,
                    Address = x.Address
                });
            });

            return cacheResult.GetDataTableResult(tableModel, true);
        }

        public async Task<DataTablesResponse> GetPendingApprovals(DataTablesModel model)
        {
            using (var context = DataContextFactory.CreateReadOnlyContext())
            {
                var query = context.ApprovalQueue
                    .Where(a => a.Type == ApprovalQueueType.WithdrawalReprocessing && a.Status == ApprovalQueueStatus.Pending)
                    .Select(x => new
                    {
                        Id = x.Id,
                        Type = x.Type,
                        User = x.DataUser.UserName,
                        RequestBy = x.RequestUser.UserName,
                        Requested = x.Created,
                        Status = x.Status,
                        Approved = x.Approved,
                        ApprovedBy = x.ApproveUser == null ? string.Empty : x.ApproveUser.UserName,
                    }).OrderByDescending(x => x.Id);

                return await query.GetDataTableResultNoLockAsync(model);
            }
        }

        public async Task<ReprocessingApprovalsModel> GetApproval(int id)
        {
            using (var context = DataContextFactory.CreateReadOnlyContext())
            {
                return await context.ApprovalQueue
                    .AsNoTracking()
                    .Where(a => a.Id == id)
                    .Select(x => new ReprocessingApprovalsModel
                    {
                        Id = x.Id,
                        Type = x.Type,
                        RequestedBy = x.RequestUser.UserName,
                        Requested = x.Created,
                        Status = x.Status,
                        Approved = x.Approved,
                        ApprovedBy = x.ApproveUser == null ? string.Empty : x.ApproveUser.UserName
                    }).FirstOrDefaultNoLockAsync(x => x.Id == id).ConfigureAwait(false);
            }
        }
    }
}
