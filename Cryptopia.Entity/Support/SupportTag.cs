using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Entity.Support
{
	public class SupportTag
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public virtual ICollection<SupportTicket> Tickets { get; set; }
	}
}
