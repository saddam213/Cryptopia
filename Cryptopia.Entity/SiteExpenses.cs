using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Entity
{
	public class SiteExpense
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string Name { get; set; }
		public decimal Price { get; set; }
		public bool IsEnabled { get; set; }
	}
}