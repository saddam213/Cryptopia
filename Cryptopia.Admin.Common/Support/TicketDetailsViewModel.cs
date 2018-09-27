using System.Collections.Generic;

namespace Cryptopia.Admin.Common.Support
{
	public class TicketDetailsViewModel
	{
		public SupportTicketModel Ticket { get; set; }
		public List<SupportMessageModel> Messages { get; set; }
	}
}
