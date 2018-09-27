using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class Address
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public Guid UserId { get; set; }
		public int CurrencyId { get; set; }

		[MaxLength(128)]
		[Column("Address")]
		public string AddressHash { get; set; }

		[MaxLength(256)]
		public string PrivateKey { get; set; }

		[ForeignKey("UserId")]
		public virtual Entity.User User { get; set; }

		[ForeignKey("CurrencyId")]
		public virtual Currency Currency { get; set; }
	}
}