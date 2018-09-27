using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Referral
{
	public class ReferralInfoModel
	{
		public decimal ActivityBonus { get; set; }
		public int Id { get; set; }
		public DateTime LastUpdate { get; set; }
		public int RefCount { get; set; }
		public string Referrer { get; set; }
		public int RoundId { get; set; }
		public ReferralStatus Status { get; set; }
		public decimal TradeFeeAmount { get; set; }
		public decimal TradePercent { get; set; }
		public int TransferId { get; set; }
	}
}
