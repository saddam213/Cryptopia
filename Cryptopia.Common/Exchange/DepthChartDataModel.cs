using System.Collections.Generic;

namespace Cryptopia.Common.Exchange
{
	public class DepthChartDataModel
	{
		public DepthChartDataModel()
		{
			SellDepth = new List<decimal[]>();
			BuyDepth = new List<decimal[]>();
		}

		public List<decimal[]> SellDepth { get; set; }
		public List<decimal[]> BuyDepth { get; set; }
	}
}