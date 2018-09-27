using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.MineshaftNotification
{
	public class WorkerNotification : IMineshaftNotification
	{
		public double Difficulty { get; set; }
		public double Hashrate { get; set; }
		public int Id { get; set; }
		public string Name { get; set; }
		public string MiningHandle { get; set; }

		public int PoolId { get; set; }
		public MineshaftDataNotificationType Type
		{
			get { return MineshaftDataNotificationType.UserWorkerStatistics; }
		}

		public Guid? UserId { get; set; }
	}
}
