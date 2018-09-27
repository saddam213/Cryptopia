using Cryptopia.Enums;

namespace Cryptopia.Admin.Common.AdminUser
{
	public class AdminResetTwoFactorModel
	{
		public int ApprovalId { get; set; }
		public string Message { get; set; }
		public ApprovalQueueStatus Status { get; set; }
		public string Type { get; set; }
		public string UserName { get; set; }
	}
}
