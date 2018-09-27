using System;

namespace Cryptopia.Common.User
{
	public class UserMessageModel
	{
		public int Id { get; set; }
		public DateTime Timestamp { get; set; }
		public string Sender { get; set; }
		public string Subject { get; set; }
		public string Message { get; set; }
		public bool IsInbound { get; set; }
		public bool IsRead { get; set; }
		public string Recipiants { get; set; }
	}
}
