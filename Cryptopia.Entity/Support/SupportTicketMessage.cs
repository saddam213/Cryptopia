namespace Cryptopia.Entity.Support
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class SupportTicketMessage
	{
		[Key]
		public int Id { get; set; }

		public string SenderId { get; set; }

		public int TicketId { get; set; }

		public string Message { get; set; }

		public bool IsInternal { get; set; }

		public bool IsDraft { get; set; }

		public DateTime LastUpdate { get; set; }

		public DateTime Created { get; set; }

		[ForeignKey("TicketId")]
		public virtual SupportTicket Ticket { get; set; }

		[ForeignKey("SenderId")]
		public virtual ApplicationUser Sender { get; set; }

		public static SupportTicketMessage CreateMessage(string userId, int ticketId, string message)
		{
			return new SupportTicketMessage
			{
				IsInternal = false,
				IsDraft = true,
				Created = DateTime.UtcNow,
				LastUpdate = DateTime.UtcNow,
				Message = message,
				SenderId = userId,
				TicketId = ticketId
			};
		}

		public static SupportTicketMessage CreateAdminMessage(string userId, int ticketId, string message)
		{
			return new SupportTicketMessage
			{
				IsInternal = true,
				IsDraft = false,
				Created = DateTime.UtcNow,
				LastUpdate = DateTime.UtcNow,
				Message = message,
				SenderId = userId,
				TicketId = ticketId
			};
		}
	}
}