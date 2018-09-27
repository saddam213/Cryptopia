using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class Location
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public int? ParentId { get; set; }

		[MaxLength(128)]
		public string Name { get; set; }

		[MaxLength(50)]
		public string CountryCode { get; set; }


		[ForeignKey("ParentId")]
		public virtual Entity.Location Parent { get; set; }
	}
}