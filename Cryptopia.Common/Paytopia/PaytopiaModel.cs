using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Paytopia
{
	public class PaytopiaModel
	{
		public PaytopiaItemModel ComboListing { get; set; }
		public PaytopiaItemModel PoolListing { get; set; }
		public PaytopiaItemModel ExchangeListing { get; set; }
		public PaytopiaItemModel FeaturedCurrency { get; set; }
		public PaytopiaItemModel FeaturedPool { get; set; }
		public PaytopiaItemModel RewardSlot { get; set; }
		public PaytopiaItemModel TipSlot { get; set; }
		public PaytopiaItemModel LottoSlot { get; set; }
		public PaytopiaItemModel Shares { get; set; }
		public PaytopiaItemModel TwoFactor { get; set; }
	}
}
