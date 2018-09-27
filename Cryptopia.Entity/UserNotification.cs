using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class UserNotification
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }

		[MaxLength(50)]
		public string Type { get; set; }

		[MaxLength(512)]
		public string Title { get; set; }

		[MaxLength(2048)]
		public string Notification { get; set; }

		public bool Acknowledged { get; set; }

		public DateTime Timestamp { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }
	}
}