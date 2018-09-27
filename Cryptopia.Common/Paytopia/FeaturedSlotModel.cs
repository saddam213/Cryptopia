using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Paytopia
{
	public class FeaturedSlotModel
	{
		[Required]
		public int? ItemId { get; set; }
		public List<FeaturedSlotItemModel> Items { get; set; }
		public decimal Balance { get; set; }
		public string Currency { get; set; }
		public decimal Price { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool IsAnonymous { get; set; }
	}

	public class FeaturedSlotItemModel
	{
		public int Id { get; set; }
		public bool IsFeatured { get; set; }
		public string Name { get; set; }
		public DateTime NextSlotBegin { get; set; }
		public DateTime NextSlotEnd { get; set; }
		public int NextSlotWeek { get; set; }
		public string SlotSummary { get; set; }
	}
}
