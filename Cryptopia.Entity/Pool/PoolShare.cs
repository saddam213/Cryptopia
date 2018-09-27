using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Entity
{
	public class PoolShare
	{
		public int Id { get; set; }
		public Guid UserId { get; set; }
		public string IPAddress { get; set; }
		public string WorkerName { get; set; }
		public double Difficulty { get; set; }
		public bool IsValidShare { get; set; }
		public bool IsValidBlock { get; set; }
		public string BlockHash { get; set; }
		public string Error { get; set; }
		public DateTime Timestamp { get; set; }
		public bool IsProcessed { get; set; }
	}
}
