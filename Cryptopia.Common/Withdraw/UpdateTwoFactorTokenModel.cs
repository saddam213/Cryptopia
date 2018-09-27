using Cryptopia.Enums;

namespace Cryptopia.Common.Withdraw
{
	public class UpdateTwoFactorTokenModel
	{
		public string Address { get; set; }
		public AddressType AddressType { get; set; }
		public decimal Amount { get; set; }
		public string Symbol { get; set; }
		public string TwoFactorToken { get; set; }
		public int WithdrawId { get; set; }
	}
}
