using System;
using Cryptopia.Enums;

namespace Cryptopia.Admin.Common.Support
{
	using System.Collections.Generic;

	public class SupportTicketBasicInfoModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public DateTime Created { get; set; }
	}
}