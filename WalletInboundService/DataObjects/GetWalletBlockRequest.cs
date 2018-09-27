namespace Cryptopia.InboundService.DataObjects
{
	public class GetWalletBlockRequest
    {
        public int CurrencyId { get; set; }
        public string BlockHash { get; set; }
        public int BlockHeight { get; set; }
    }
}
