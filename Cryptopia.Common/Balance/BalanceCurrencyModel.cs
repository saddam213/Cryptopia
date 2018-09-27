namespace Cryptopia.Common.Balance
{
	public class BalanceCurrencyModel
	{
		public int CurrencyId { get; set; }
		public string Symbol { get; set; }
		public decimal Total { get; set; }
		public decimal HeldForTrades { get; set; }
		public decimal PendingWithdraw { get; set; }
		public decimal Unconfirmed { get; set; }
		public decimal MinTipAmount { get; set; }

		public decimal Available
		{
			get { return Total - (Unconfirmed + PendingWithdraw + HeldForTrades); }
		}
	}
}
