using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class ForumReport
	{
		public ForumReport()
		{
			Timestamp = DateTime.UtcNow;
		}

		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }

		public int PostId { get; set; }

		[MaxLength(1000)]
		public string Message { get; set; }

		public DateTime Timestamp { get; set; }

		[ForeignKey("PostId")]
		public virtual ForumPost Post { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }
	}
}