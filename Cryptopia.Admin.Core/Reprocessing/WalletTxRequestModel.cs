
namespace Web.Admin.Models.Reprocessing
{
    public class WalletTxRequestModel
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        public string Address { get; set; }
        public string BlockLength { get; set; }
    }
}