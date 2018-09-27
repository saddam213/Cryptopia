using Cryptopia.Enums;

namespace Cryptopia.Common.Trade
{
	public class CreateWithdrawModel
	{
		public string Address { get; set; }
		public decimal Amount { get; set; }
		public int CurrencyId { get; set; }
		public string TwoFactorToken { get; set; }
		public WithdrawType Type { get; set; }
	}
}
