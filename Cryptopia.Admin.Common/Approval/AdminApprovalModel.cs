using System;
using Cryptopia.Enums;

namespace Cryptopia.Admin.Common.Approval
{
	public class AdminApprovalModel
	{
		public int Id { get; set; }
		public string DataUser { get; set; }
		public ApprovalQueueType Type { get; set; }
		public string Data { get; set; }
		public DateTime Created { get; set; }
		public string RequestUser { get; set; }
		public ApprovalQueueStatus Status { get; set; }
		public string ApprovalUser { get; set; }
		public DateTime Approved { get; set; }
		public string Message { get; set; }
	}
}
