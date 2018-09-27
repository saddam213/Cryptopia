namespace Cryptopia.InboundService.DataObjects
{
	public class GetWalletTransactionRequest
    {
        public int CurrencyId { get; set; }
        public string TxId { get; set; }
    }
}
