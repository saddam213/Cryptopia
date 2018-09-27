using System;
using Cryptopia.Enums;

namespace Cryptopia.Admin.Common.AdminUser
{
	public class AdminChangeEmailApproveModel
	{
		public int ApprovalId { get; set; }
		public string UserName { get; set; }
		public string OldEmailAddress { get; set; }
		public string NewEmailAddress { get; set; }


		
		public string Requestor { get; set; }
		public DateTime Requested { get; set; }

		public string Approver { get; set; }
		public DateTime Approved { get; set; }

		public ApprovalQueueStatus Status { get; set; }
		public string Message { get; set; }

	}
}
