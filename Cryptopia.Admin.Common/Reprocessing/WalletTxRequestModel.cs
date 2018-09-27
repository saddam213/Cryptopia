
namespace Cryptopia.Admin.Common.Reprocessing
{
    public class WalletTxRequestModel
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        public string Address { get; set; }
        public int BlockLength { get; set; }
    }
}