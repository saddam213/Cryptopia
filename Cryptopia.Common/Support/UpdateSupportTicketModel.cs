using Cryptopia.Enums;

namespace Cryptopia.Common.Support
{
	public class UpdateSupportTicketModel
	{
		public SupportTicketStatus Status { get; set; }
		public int TicketId { get; set; }
		public string Timestamp { get; set; }
		public bool IsAdmin { get; set; }
	}
}
