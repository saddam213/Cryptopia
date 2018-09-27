namespace Cryptopia.Entity.Support
{
	using Cryptopia.Enums;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class SupportTicket
	{
		[Key]
		public int Id { get; set; }

		public string UserId { get; set; }

		public int QueueId { get; set; }

		[MaxLength(256)]
		public string Title { get; set; }

		[MaxLength(4000)]
		public string Description { get; set; }

		[Column("StatusId")]
		public SupportTicketStatus Status { get; set; }

		[Column("CategoryId")]
		public SupportTicketCategory Category { get; set; }

		public DateTime LastUpdate { get; set; }
		public DateTime Created { get; set; }

		[ForeignKey("QueueId")]
		public virtual SupportTicketQueue Queue { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }
		public virtual ICollection<SupportTicketMessage> Messages { get; set; }
		public virtual ICollection<SupportTag> Tags { get; set; }
	}
}