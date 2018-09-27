using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Enums;

namespace Cryptopia.Common.MineshaftNotification
{
	public class BlockNotification : IMineshaftNotification
	{
		public decimal Amount { get; set; }
		public int Confirmations { get; set; }
		public double Difficulty { get; set; }
		public string Finder { get; set; }
		public int Height { get; set; }
		public double Luck { get; set; }
		public string Status { get; set; }
		public DateTime Timestamp { get; set; }

		public int PoolId { get; set; }
		public MineshaftDataNotificationType Type
		{
			get { return MineshaftDataNotificationType.Block; }
		}

		public Guid? UserId { get; set; }
		public double Shares { get; set; }
	}
}
