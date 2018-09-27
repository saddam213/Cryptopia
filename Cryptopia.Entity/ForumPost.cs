using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class ForumPost
	{
		public ForumPost()
		{
			Timestamp = DateTime.UtcNow;
			LastUpdate = DateTime.UtcNow;
		}

		[Key]
		public int Id { get; set; }

		public int ThreadId { get; set; }

		public string Message { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }

		public bool IsFirstPost { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime LastUpdate { get; set; }

		public DateTime Timestamp { get; set; }

		[ForeignKey("ThreadId")]
		public virtual ForumThread Thread { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }

		
	}
}