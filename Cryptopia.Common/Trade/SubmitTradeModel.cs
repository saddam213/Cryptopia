namespace Cryptopia.Common.Trade
{
	public class SubmitTradeModel
	{
		public int TradePairId { get; set; }
		//public int CurrencyId1 { get; set; }
		//public int CurrencyId2 { get; set; }
		public decimal Price { get; set; }
		public decimal Amount { get; set; }
		public bool IsBuy { get; set; }
	}
}
