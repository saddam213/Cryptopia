using System;

namespace Cryptopia.API.DataObjects
{
	public class PoolShare
	{
		public int Id { get; set; }
		public string RemHost { get; set; }
		public bool OurResult { get; set; }
		public bool UpstreamResult { get; set; }
		public string Reason { get; set; }
		public string BlockHash { get; set; }
		public double Difficulty { get; set; }
		public string WorkerName { get; set; }
		public DateTime Time { get; set; }
		public int Port { get; set; }
	}
}
