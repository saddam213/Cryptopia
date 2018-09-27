namespace Cryptopia.Common.Balance
{
	public class BalanceTradePairModel
	{
		public int TradePairId { get; set; }
		public string Symbol { get; set; }
		public string BaseSymbol { get; set; }

		public decimal Available { get; set; }
		public decimal BaseAvailable { get; set; }
		public decimal HeldForOrders { get; set; }
		public decimal BaseHeldForOrders { get; set; }
	}
}
