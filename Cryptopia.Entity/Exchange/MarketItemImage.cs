using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class MarketItemImage
	{
		[Key]
		public int Id { get; set; }

		public int MarketItemId { get; set; }

		[MaxLength(256)]
		public string Image { get; set; }


		[ForeignKey("MarketItemId")]
		public virtual Entity.MarketItem MarketItem { get; set; }
	}
}