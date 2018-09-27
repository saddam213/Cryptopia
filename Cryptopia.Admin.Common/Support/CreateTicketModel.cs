using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.Support
{
	public class CreateTicketModel
	{
		[Required]
		public int QueueId { get; set; }
		[Required]
		public string UserName { get; set; }
		[Required]
		public string Title { get; set; }
		[Required]
		public string Description { get; set; }
		[Required]
		public SupportTicketCategory Category { get; set; }

		public Dictionary<int, string> QueueDictionary { get; set; }
		public Dictionary<SupportTicketCategory, string> CategoryDictionary { get; set; }
	}
}
