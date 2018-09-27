using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.MineshaftNotification
{
	public class UserStatisticNotification : IMineshaftNotification
	{
		public double Hashrate { get; set; }
		public double InvalidShares { get; set; }
		public string MiningHandle { get; set; }
		public double ValidShares { get; set; }

		public int PoolId { get; set; }
		public MineshaftDataNotificationType Type
		{
			get { return MineshaftDataNotificationType.UserStatistics; }
		}

		public Guid? UserId { get; set; }
		public int WorkerCount { get; set; }
	}
}
