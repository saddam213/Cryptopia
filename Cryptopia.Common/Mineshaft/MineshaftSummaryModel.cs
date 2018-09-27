using Cryptopia.Enums;

namespace Cryptopia.Common.Mineshaft
{
	public class MineshaftSummaryModel
	{
		public int PoolId { get; set; }
		public double Hashrate { get; set; }
		public string Symbol { get; set; }
		public AlgoType AlgoType { get; set; }
		public string Name { get; set; }
		public int Miners { get; set; }
		public int BlocksFound { get; set; }
		public double Luck { get; set; }
	}
}
