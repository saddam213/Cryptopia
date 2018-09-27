using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class UserKarmaHistory
	{
		public UserKarmaHistory()
		{
			Timestamp = DateTime.UtcNow;
		}

		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }

		public int Amount { get; set; }

		[MaxLength(256)]
		public string TxId { get; set; }
	
		public DateTime Timestamp { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }
	}
}