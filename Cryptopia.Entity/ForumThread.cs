using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class ForumThread
	{
		public ForumThread()
		{
			Timestamp = DateTime.UtcNow;
			LastUpdate = DateTime.UtcNow;
		}

		[Key]
		public int Id { get; set; }

		public int CategoryId { get; set; }

		[MaxLength(256)]
		public string Title { get; set; }

		[MaxLength(512)]
		public string Description { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }

		[MaxLength(128)]
		public string Icon { get; set; }

		public bool IsPinned { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime LastUpdate { get; set; }

		public DateTime Timestamp { get; set; }

		public virtual ICollection<ForumPost> Posts { get; set; }
		
		[ForeignKey("CategoryId")]
		public virtual ForumCategory Category { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }
	}
}