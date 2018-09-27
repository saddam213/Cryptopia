namespace Cryptopia.RewardService.Implementation
{
	public class RewardItem
	{
		public int Currency { get; set; }
		public decimal Amount { get; set; }
		public string Message { get; set; }
		public decimal Percent { get; set; }
		public RewardType RewardType { get; set; }
	}
}
