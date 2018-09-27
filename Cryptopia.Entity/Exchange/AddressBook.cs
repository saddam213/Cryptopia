using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class AddressBook
	{
		[Key]
		public int Id { get; set; }

		public Guid UserId { get; set; }

		public int CurrencyId { get; set; }

		[MaxLength(128)]
		public string Label { get; set; }

		[MaxLength(512)]
		public string Address { get; set; }

		public bool IsEnabled { get; set; }


		[ForeignKey("UserId")]
		public virtual Entity.User User { get; set; }

		[ForeignKey("CurrencyId")]
		public virtual Currency Currency { get; set; }
	}
}