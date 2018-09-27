using Cryptopia.Common.Validation;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Common.Support
{
	public class CreateSupportReplyModel
	{
		[RequiredBase]
		public int TicketId { get; set; }

		[RequiredBase]
		[Display(Name = nameof(Resources.Support.MessageLabel), ResourceType = typeof(Resources.Support))]
		public string Message { get; set; }

		public string Sender { get; set; }
		public string Timestamp { get; set; }
		public string Email { get; set; }
		public string UserId { get; set; }
		public string UserName { get; set; }
	}
}
