using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Site.Models
{
	public class ChatPayBanModel
	{
		public int Seconds { get; set; }
		public decimal Balance { get; set; }
		public string ChatHandle { get; set; }
		public List<string> ChatHandles { get; set; }
		public decimal PayBanPrice { get; set; }

		[MaxLength(500)]
		public string Reason { get; set; }
	}
}