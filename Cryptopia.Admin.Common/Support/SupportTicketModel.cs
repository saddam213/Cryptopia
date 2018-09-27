using System;
using Cryptopia.Enums;

namespace Cryptopia.Admin.Common.Support
{
	using System.Collections.Generic;

	public class SupportTicketModel
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Status { get; set; }
		public string Category { get; set; }
		public string Queue { get; set; }
		public DateTime LastUpdate { get; set; }
		public DateTime Created { get; set; }
		public List<string> Tags { get; set; }
		public List<SupportTicketBasicInfoModel> OpenTickets { get; set; }
	}
}