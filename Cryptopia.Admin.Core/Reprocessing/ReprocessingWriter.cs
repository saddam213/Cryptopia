using System;
using System.Linq;
using System.Threading.Tasks;

using Cryptopia.Admin.Common.Reprocessing;
using Cryptopia.Common.Cache;
using Cryptopia.Common.DataContext;
using Cryptopia.Entity;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.Results;
using Cryptopia.Infrastructure.Common.DataContext;
using Newtonsoft.Json;

namespace Cryptopia.Admin.Core.Reprocessing
{
    public class ReprocessingWriter : IReprocessingWriter
    {
        public IDataContextFactory DataContextFactory { get; set; }
        public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

        public async Task<IWriterResult> UpdateWithrawalTransactionId(string adminUserId, UpdateWithdrawalTxModel model)
        {
            Withdraw existing = null;

            using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
            {
                existing = await context.Withdraw.FirstOrDefaultNoLockAsync(x => x.Id.Equals(model.Id)).ConfigureAwait(false);

                if (existing == null)
                    return new WriterResult(false, "No Withdrawal found to update.");

                if (model.Status != Cryptopia.Enums.WithdrawStatus.Processing)
                    return new WriterResult(false, "Withdrawal in incorrect state to update.");

                if (!model.Address.Equals(existing.Address))
                    return new WriterResult(false, "Original and updated withdrawal addresses are different.");

                if (!model.Amount.Equals(existing.Amount))
                    return new WriterResult(false, "Original and updated withdrawal amounts are different.");
            }

            // post the request to the approval queue
            using (var approvalContext = DataContextFactory.CreateContext())
            {
                try
                {
                    string withdrawalId = $"{model.Id}";
                    string withdrawalUserId = $"{existing.UserId}";
                    var approval = await approvalContext.ApprovalQueue.FirstOrDefaultNoLockAsync(a => a.DataUserId == withdrawalUserId  
                                                                                                 && a.Type == ApprovalQueueType.WithdrawalReprocessing 
                                                                                                 && a.Status == ApprovalQueueStatus.Pending
                                                                                                 && a.Data == withdrawalId);

                    if (approval != null)
                        return new WriterResult(false, "Already awaiting approval.");

                    approval = new Entity.ApprovalQueue
                    {
                        DataUserId = $"{existing.UserId}",
                        RequestUserId = adminUserId,
                        Type = ApprovalQueueType.WithdrawalReprocessing,
                        Status = ApprovalQueueStatus.Pending,
                        Created = DateTime.UtcNow,
                        Data = JsonConvert.SerializeObject(new ReprocessingApprovalDataModel { WithdrawalId = $"{model.Id}", TxId = model.TxId })
                    };

                    approvalContext.ApprovalQueue.Add(approval);
                    await approvalContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new WriterResult(false, "Failed to add request to Approval Queue.");
                }

                return new WriterResult(true, $"Successfully added Withdrawal: {existing.Id} to Approval Queue.");
            }
        }

        public async Task<IWriterResult> ApproveWithdrawalReprocessing(string adminUserId, ReprocessingApprovalsModel model)
        {
            ReprocessingApprovalDataModel dataModel = null;
            ApprovalQueue approval = null;

            using (var context = DataContextFactory.CreateContext())
            {
                approval = await context.ApprovalQueue.FirstOrDefaultNoLockAsync(a => a.Id == model.Id);

                if (approval == null)
                    return new WriterResult(false, "Unable to find approval to update.");

                if (approval.Status != ApprovalQueueStatus.Pending)
                {
                    if (approval.Status == ApprovalQueueStatus.Approved)
                        return new WriterResult(false, "Already approved");

                    return new WriterResult(false, $"Unable to update approval from {approval.Status}.");
                }

                if (adminUserId.Equals(approval.RequestUserId))
                    return new WriterResult(false, "Cannot approve your own request.");

                try
                {
                    dataModel = JsonConvert.DeserializeObject<ReprocessingApprovalDataModel>(approval.Data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new WriterResult(false, "Unable to complete approval update.");
                }

                approval.Approved = DateTime.UtcNow;
                approval.ApproveUserId = adminUserId;
                approval.Status = ApprovalQueueStatus.Approved;
                await context.SaveChangesAsync();
            }

            // ToDo: move to method used to complete approval and commit change to database
            using (var context = ExchangeDataContextFactory.CreateContext())
            {
                int withdrawalId;
                if (!int.TryParse(dataModel.WithdrawalId, out withdrawalId))
                    return new WriterResult(false, $"Invalid Withdrawal Id: {dataModel.WithdrawalId}");

                if (withdrawalId == 0 || withdrawalId < 0)
                    return new WriterResult(false, "Invalid Withdrawal Id supplied.");

                var withdrawal = await context.Withdraw
                    .Where(w => w.Id == withdrawalId)
                    .FirstOrDefaultNoLockAsync().ConfigureAwait(false);

                if (withdrawal == null)
                    return new WriterResult(false, $"Withdrawal {dataModel.WithdrawalId} not found.");

                withdrawal.Txid = dataModel.TxId;
                withdrawal.Status = WithdrawStatus.Complete;
                withdrawal.RetryCount++;
                await context.SaveChangesAsync().ConfigureAwait(false);
                await context.AuditUserBalance(withdrawal.UserId, withdrawal.CurrencyId);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }

            return new WriterResult(true, "Successfully updated withdrawal.");
        }
    }
}
