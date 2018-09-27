using System;

namespace Cryptopia.API.DataObjects
{
	public class PoolBlock
	{
		public int Id { get; set; }
		public int Height { get; set; }
		public string BlockHash { get; set; }
		public int Confirmations { get; set; }
		public PoolBlockStatus Status { get; set; }
		public decimal Reward { get; set; }
		public decimal Fee { get; set; }
		public string WorkerName { get; set; }
		public int Shares { get; set; }
		public double Luck { get; set; }
		public double Difficulty { get; set; }
		public int ShareId { get; set; }
		public int Port { get; set; }
		public string Symbol { get; set; }
		public DateTime Time { get; set; }
	}
}
