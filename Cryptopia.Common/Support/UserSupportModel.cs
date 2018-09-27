using System.Collections.Generic;

namespace Cryptopia.Common.Support
{
	public class UserSupportModel
	{
		public UserSupportModel()
		{
			SupportTickets = new List<SupportTicketModel>();
		}
		public List<SupportTicketModel> SupportTickets { get; set; }
	}
}
