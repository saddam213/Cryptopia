using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Entity.Support
{
	public class SupportTicketQueue
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public int Order { get; set; }

        public bool IsDeleted { get; set; }

		public virtual ICollection<SupportTicket> Tickets { get; set; }
	}
}
