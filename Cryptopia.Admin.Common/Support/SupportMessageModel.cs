using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Admin.Common.Support
{
	using System;

	public class SupportMessageModel
	{
		public int Id { get; set; }
		public int TicketId { get; set; }
		public string Message { get; set; }
		public bool IsInternal { get; set; }
		public bool IsDraft { get; set; }

		public string SenderId { get; set; }
		public string Email { get; set; }
		public string UserName { get; set; }

		public DateTime LastUpdate { get; set; }
		public DateTime Created { get; set; }
	}
}