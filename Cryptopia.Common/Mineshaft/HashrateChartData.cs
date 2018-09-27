using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Mineshaft
{
	public class HashrateChartData
	{
		public DateTime Timestamp { get; set; }
		public DateTime LastTime { get; set; }
		public double Shares { get; set; }
	}
}
