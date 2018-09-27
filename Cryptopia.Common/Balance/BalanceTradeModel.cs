namespace Cryptopia.Common.User
{
	public class BalanceTradeModel
	{
		public int CurrencyId { get; set; }
		public string Symbol { get; set; }
		public decimal Available
		{
			get { return Total - (Unconfirmed + HeldForTrades + PendingWithdraw); }
		}
		public decimal HeldForTrades { get; set; }
		public decimal EstimatedBTC { get; set; }
	

		public decimal Total { get; set; }
		public decimal Unconfirmed { get; set; }
		public decimal PendingWithdraw { get; set; }
		
		public bool Favorite { get; set; }
	}
}