namespace Cryptopia.Common.Trade
{
	public class CreateTradeModel
	{
		public decimal Amount { get; set; }
		public bool IsBuy { get; set; }
		public decimal Price { get; set; }
		public int TradePairId { get; set; }
	}
}