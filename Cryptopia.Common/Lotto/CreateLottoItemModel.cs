using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cryptopia.Enums;
using Cryptopia.Common.Currency;

namespace Cryptopia.Common.Lotto
{
	public class CreateLottoItemModel
	{
		public CreateLottoItemModel()
		{
			NextDraw = DateTime.UtcNow;
		}

		[Required]
		[StringLength(128)]
		public string Name { get; set; }

		[Required]
		[StringLength(4000)]
		public string Description { get; set; }

		public int CurrencyId { get; set; }

		[Range(1, 9)]
		public int Prizes { get; set; }

		[Range(typeof (decimal), "0.00000001", "1000000000")]
		public decimal Rate { get; set; }

		[Range(typeof (decimal), "0.00000000", "1000000000")]
		public decimal Fee { get; set; }

		[Range(0, 8760)]
		public int Hours { get; set; }

		public LottoType LottoType { get; set; }
		public LottoItemStatus Status { get; set; }
		public DateTime NextDraw { get; set; }
		public List<CurrencyModel> Currencies { get; set; }
		public DateTime Expires { get; set; }
	}
}