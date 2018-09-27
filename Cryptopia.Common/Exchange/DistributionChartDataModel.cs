using System.Collections.Generic;

namespace Cryptopia.Common.Exchange
{
	public class DistributionChartDataModel
	{
		public DistributionChartDataModel()
		{
			Distribution = new List<decimal>();
		}

		public List<decimal> Distribution { get; set; }
	}
}
