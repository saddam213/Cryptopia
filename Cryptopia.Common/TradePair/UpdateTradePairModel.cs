using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.TradePair
{
	public class UpdateTradePairModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public TradePairStatus Status { get; set; }
		public string StatusMessage { get; set; }
	}
}
