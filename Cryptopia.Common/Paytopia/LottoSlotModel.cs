using Cryptopia.Common.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Paytopia
{
	public class LottoSlotModel
	{
		public decimal Balance { get; set; }
		public string Currency { get; set; }
		public string Description { get; set; }

		[Display(Name = nameof(Resources.Paytopia.lottoCurrencyLabel), ResourceType = typeof(Resources.Paytopia))]
		public int ItemId { get; set; }

		public List<LottoSlotItemModel> Items { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }

		[RangeBase(typeof(decimal),"0.00000001","99999999999")]
		[Display(Name = nameof(Resources.Paytopia.lottoTicketPriceLabel), ResourceType = typeof(Resources.Paytopia))]
		public decimal LottoPrice { get; set; }

		[RequiredBase]
		[StringLengthBase(40, MinimumLength =5)]
		[Display(Name = nameof(Resources.Paytopia.lottoGameTitleLabel), ResourceType = typeof(Resources.Paytopia))]
		public string LottoName { get; set; }

		[StringLengthBase(250)]
		[Display(Name = nameof(Resources.Paytopia.lottoDescriptionLabel), ResourceType = typeof(Resources.Paytopia))]
		public string LottoDescription { get; set; }
	}

	public class LottoSlotItemModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}
