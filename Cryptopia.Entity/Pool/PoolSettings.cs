using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cryptopia.Enums;
using System;

namespace Cryptopia.Entity
{
	public class PoolSettings
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
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