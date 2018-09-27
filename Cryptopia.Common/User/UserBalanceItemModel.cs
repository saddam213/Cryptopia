using Cryptopia.Enums;

namespace Cryptopia.Common.User
{
	public class UserBalanceItemModel
	{
		public int CurrencyId { get; set; }
		public string Symbol { get; set; }
		public string Name { get; set; }
		public decimal Total { get; set; }
		public decimal Unconfirmed { get; set; }
		public decimal PoolPending { get; set; }
		public decimal HeldForTrades { get; set; }
		public decimal PendingWithdraw { get; set; }
		public string Address { get; set; }
		public CurrencyStatus Status { get; set; }
		public CurrencyListingStatus ListingStatus { get; set; }
		public string StatusMessage { get; set; }
		public bool IsFavorite { get; set; }

		public decimal Available
		{
			get { return Total - (Unconfirmed + HeldForTrades + PendingWithdraw + PoolPending); }
		}

		public CurrencyType CurrencyType { get; set; }
		public decimal EstimatedBTC { get; set; }
		public string BaseAddress { get; set; }
	}
}