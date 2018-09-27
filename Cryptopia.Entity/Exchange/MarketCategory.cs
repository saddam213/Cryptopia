using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class MarketCategory
	{
		[Key]
		public int Id { get; set; }
		public int? ParentId { get; set; }

		[MaxLength(128)]
		public string Name { get; set; }

		[MaxLength(128)]
		public string DisplayName { get; set; }

		[ForeignKey("ParentId")]
		public virtual Entity.MarketCategory Parent { get; set; }
	}
}
