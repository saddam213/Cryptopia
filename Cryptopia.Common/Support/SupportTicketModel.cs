using Cryptopia.Enums;
using System;
using System.Collections.Generic;

namespace Cryptopia.Common.Support
{
	public class SupportTicketModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public SupportTicketStatus Status { get; set; }
		public SupportTicketCategory Category { get; set; }
		public DateTime LastUpdate { get; set; }
		public DateTime Created { get; set; }
		public List<SupportTicketMessageModel> Messages { get; set; }
	}
}
