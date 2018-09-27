using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.TradePair
{
	public class TradePairTickerModel
	{
		public int Id { get; set; }
		public decimal Low { get; set; }
		public decimal High { get; set; }
		public decimal Last { get; set; }
		public decimal Change { get; set; }
		public decimal Volume { get; set; }
		public decimal BaseVolume { get; set; }

	}
}
