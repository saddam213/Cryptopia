using System;
using Cryptopia.Enums;

namespace Cryptopia.Admin.Common.AdminUser
{
	public class AdminResetTwoFactorApproveModel
	{
		public int ApprovalId { get; set; }
		public string Requestor { get; set; }
		public DateTime Requested { get; set; }

		public ApprovalQueueStatus Status { get; set; }
		public string Type { get; set; }
		public string UserName { get; set; }
		public string Message { get; set; }
	}
}
