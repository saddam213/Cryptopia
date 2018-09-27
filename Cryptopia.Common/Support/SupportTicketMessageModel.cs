using System;

namespace Cryptopia.Common.Support
{
	public class SupportTicketMessageModel
	{
		public int Id { get; set; }
		public string Sender { get; set; }
		public string Message { get; set; }
		public bool IsAdminReply { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}
