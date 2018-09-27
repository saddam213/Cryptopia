using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Mineshaft
{
	public class BlockChartModel
	{
		public BlockChartModel()
		{
			BlockHeightData = new List<int>();
			EstimatedData = new List<double>();
			ActualData = new List<double>();
		}

		public List<int> BlockHeightData { get; set; }
		public List<double> ActualData { get; set; }
		public List<double> EstimatedData { get; set; }
	}
}
