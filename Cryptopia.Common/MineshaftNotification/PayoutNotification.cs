using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Enums;

namespace Cryptopia.Common.MineshaftNotification
{
	public class PayoutNotification : IMineshaftNotification
	{
		public decimal Amount { get; set; }
		public int Block { get; set; }
		public int Id { get; set; }

		public int PoolId { get; set; }
		public double Shares { get; set; }
		public string Status { get; set; }
		public DateTime Timestamp { get; set; }
		public int TransferId { get; set; }

		public MineshaftDataNotificationType Type
		{
			get { return MineshaftDataNotificationType.UserPayout; }
		}

		public Guid? UserId { get; set; }
	}
}
