using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Pool
{
	public class PoolSettingsModel
	{
		public bool ProcessorEnabled { get; set; }
		public int HashRateCalculationPeriod { get; set; }
		public int StatisticsPollPeriod { get; set; }
		public int PayoutPollPeriod { get; set; }
		public int SitePayoutPollPeriod { get; set; }
		public int ProfitabilityPollPeriod { get; set; }
		public bool ProfitSwitchEnabled { get; set; }
		public decimal ProfitSwitchDepthBTC { get; set; }
		public decimal ProfitSwitchDepthLTC { get; set; }
	}
}
