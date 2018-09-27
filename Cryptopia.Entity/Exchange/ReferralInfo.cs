using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Entity
{
	public class ReferralInfo
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public int RoundId { get; set; }

		public Guid UserId { get; set; }

		public int RefCount { get; set; }

		public decimal ActivityAmount { get; set; }

		public decimal TradeFeeAmount { get; set; }

		public ReferralStatus Status { get; set; }

		public int TransferId { get; set; }

		public DateTime LastUpdate { get; set; }

		public DateTime Timestamp { get; set; }

		[ForeignKey("UserId")]
		public User User { get; set; }
	
	}
}
