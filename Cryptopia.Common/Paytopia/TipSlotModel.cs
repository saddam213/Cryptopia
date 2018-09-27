using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Paytopia
{
	public class TipSlotModel
	{
		public decimal Balance { get; set; }
		public string Currency { get; set; }
		public string Description { get; set; }

		[Required]
		public int? ItemId { get; set; }
		public List<TipSlotItemModel> Items { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
	}

	public class TipSlotItemModel
	{
		public DateTime Expires { get; set; }
		public int Id { get; set; }
		public bool IsExpired { get; set; }
		public string Name { get; set; }
		public DateTime NewExpiry { get; set; }
	}
}
