using Cryptopia.Enums;

namespace Cryptopia.Admin.Common.Reprocessing
{
    public class UpdateWithdrawalTxModel
    {
        public int Id { get; set; }
        public string TxId { get; set; }
        public string Address { get; set; }
        public decimal Amount { get; set; }
        public WithdrawStatus Status { get; set; }
        public int RetryCount { get; set; }
    }
}
