using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.Support
{
	public class UpdateTicketModel
	{
		public int TicketId { get; set; }
		public int QueueId { get; set; }
		public SupportTicketCategory Category { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public Dictionary<int, string> QueueDictionary { get; set; }
		public Dictionary<SupportTicketCategory, string> CategoryDictionary { get; set; }
		public string Tags { get; set; }
	}
}
