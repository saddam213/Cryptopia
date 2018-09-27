using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class ChatMessage
	{
		public ChatMessage()
		{
			Timestamp = DateTime.UtcNow;
		}

		[Key]
		public int Id { get; set; }
		public string UserId { get; set; }
		public string Message { get; set; }
		public DateTime Timestamp { get; set; }
		public bool IsEnabled { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }
	}
}