using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class ForumCategory
	{
		public ForumCategory()
		{
			Timestamp = DateTime.UtcNow;
		}

		[Key]
		public int Id { get; set; }

		public int ForumId { get; set; }

		public string Name { get; set; }

		public int Order { get; set; }

		[MaxLength(512)]
		public string Description { get; set; }

		[MaxLength(128)]
		public string Icon { get; set; }

		public virtual ICollection<ForumThread> Threads { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime Timestamp { get; set; }

		[ForeignKey("ForumId")]
		public virtual Forum Forum { get; set; }
	}
}