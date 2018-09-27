using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Paytopia
{
	public class PaytopiaItemModel
	{
		public string Description { get; set; }
		public int Id { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
		public PaytopiaItemCategory Category { get; set; }
		public PaytopiaItemType Type { get; set; }
		public int CurrencyId { get; set; }
		public string Symbol { get; set; }
	}
}
