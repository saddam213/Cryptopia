using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.MineshaftNotification
{
	public interface IMineshaftNotification
	{
		int PoolId { get; set; }
		Guid? UserId { get; set; }
		MineshaftDataNotificationType Type { get; }
	}

	public class MineshaftNotification
	{
		public Guid? UserId { get; set; }
		public int PoolId { get; set; }
		public MineshaftDataNotificationType Type { get; set; }
		public object Data { get; set; }
	}
}
