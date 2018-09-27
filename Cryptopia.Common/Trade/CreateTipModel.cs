using System;
using System.Collections.Generic;

namespace Cryptopia.Common.Trade
{
	public class CreateTipModel
	{
		public decimal Amount { get; set; }
		public int CurrencyId { get; set; }
		public List<Guid> UserTo { get; set; }
	}
}