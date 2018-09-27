using System;

namespace Cryptopia.API.DataObjects
{
	public class PoolPayout
	{
		public int Id { get; set; }
		public string BlockHash { get; set; }
		public Guid UserId { get; set; }
		public decimal Amount { get; set; }
		public PoolPayoutType Type { get; set; }
		public PoolPayoutStatus Status { get; set; }
		public string TxId { get; set; }
		public int Port { get; set; }
		public string Symbol { get; set; }
		public DateTime Time { get; set; }
	}
}
