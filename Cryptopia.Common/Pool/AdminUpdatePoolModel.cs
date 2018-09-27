using Cryptopia.Enums;

namespace Cryptopia.Common.Pool
{
	public class AdminUpdatePoolModel
	{
		public decimal BlockReward { get; set; }
		public int BlockTime { get; set; }
		public int Id { get; set; }
		public bool IsForkCheckDisabled { get; set; }
		public string Pool { get; set; }
		public string SpecialInstructions { get; set; }
		public PoolStatus Status { get; set; }
		public string StatusMessage { get; set; }
		public decimal WalletFee { get; set; }
	}
}