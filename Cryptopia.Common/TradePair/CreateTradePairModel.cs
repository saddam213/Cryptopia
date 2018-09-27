using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.TradePair
{
	public class CreateTradePairModel
	{
		[Required]
		public int? CurrencyId1 { get; set; }

		[Required]
		public int? CurrencyId2 { get; set; }

		[Required]
		public TradePairStatus? Status { get; set; }

		[MaxLength(500)]
		public string StatusMessage { get; set; }

		public List<Currency.CurrencyModel> Currencies { get; set; }
	}
}
