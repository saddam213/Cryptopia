using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Entity
{
	public class Forum
	{
		public Forum()
		{
			Timestamp = DateTime.UtcNow;
		}

		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string Title { get; set; }

		[MaxLength(512)]
		public string Description { get; set; }

		public int Order { get; set; }

		[MaxLength(128)]
		public string Icon { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime Timestamp { get; set; }

		public virtual ICollection<ForumCategory> Categories { get; set; }
	}
}