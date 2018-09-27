using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.MineshaftNotification
{
	public class StatisticNotification : IMineshaftNotification
	{
		public double BlockProgress { get; set; }
		public int Connections { get; set; }
		public int CurrentBlock { get; set; }
		public double EstimatedShares { get; set; }
		public int EstimatedTime { get; set; }
		public double Hashrate { get; set; }
		public double InvalidShares { get; set; }
		public DateTime? LastBlockTime { get; set; }
		public int LastPoolBlock { get; set; }
		public double NetworkDifficulty { get; set; }
		public double NetworkHashrate { get; set; }
		public double ValidShares { get; set; }

		public int PoolId { get; set; }
		public MineshaftDataNotificationType Type
		{
			get { return MineshaftDataNotificationType.Statistics; }
		}

		public Guid? UserId { get; set; }
		public int UserCount { get; set; }
	}
}
