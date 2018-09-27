using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class UserMessage
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }

		[MaxLength(128)]
		public string SenderUserId { get; set; }

		public DateTime Timestamp { get; set; }

		[MaxLength(512)]
		public string Subject { get; set; }

		public string Message { get; set; }

		[MaxLength(512)]
		public string Recipients { get; set; }

		public bool IsInbound { get; set; }

		public bool IsRead { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }

		[ForeignKey("SenderUserId")]
		public virtual ApplicationUser Sender { get; set; }
	}
}