using System.Collections.Generic;

namespace Cryptopia.Common.Exchange
{
	public class StockChartDataModel
	{
   	public StockChartDataModel()
		{
			Candle = new List<decimal[]>();
			Volume = new List<VolumePoint>();
		}
		
		public List<decimal[]> Candle { get; set; }
		public List<VolumePoint> Volume { get; set; }
	}

	public class VolumePoint
	{
		public decimal x { get; set; }
		public decimal y { get; set; }
		public decimal basev { get; set; }
	}
}