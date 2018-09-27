using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Mineshaft
{
	public class HashrateChartModel
	{
		public HashrateChartModel()
		{
			PoolData = new List<double[]>();
			UserData = new List<double[]>();
		}

		public List<double[]> PoolData { get; set; }
		public List<double[]> UserData { get; set; }
	}
}
