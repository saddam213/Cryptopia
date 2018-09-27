using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Enums;

namespace Cryptopia.Common.Shareholder
{
	public class PaytopiaDatatableModel
	{
		public int Id { get; set; }
		public int CurrencyId { get; set; }
		public string Symbol { get; set; }
		public string Name { get; set; }
		public decimal Amount { get; set; }
		public PaytopiaItemType Type { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
