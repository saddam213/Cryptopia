using Cryptopia.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class UserKarma
	{
		public UserKarma()
		{
			Timestamp = DateTime.UtcNow;
		}

		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }

		[Index("IX_Discriminator", 1, IsUnique = true)]
		public UserKarmaType KarmaType { get; set; }

		[MaxLength(128)]
		[Index("IX_Discriminator", 2, IsUnique = true)]
		public string SenderId { get; set; }

		[MaxLength(128)]
		[Index("IX_Discriminator", 3, IsUnique = true)]
		public string Discriminator { get; set; }

		public DateTime Timestamp { get; set; }


		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }

		[ForeignKey("SenderId")]
		public virtual ApplicationUser Sender { get; set; }
	}
}