using Cryptopia.Enums;

namespace Cryptopia.Common.User
{
	public class UserMessageReportModel
	{
		public int MessageId { get; set; }
		public ReportReason Reason { get; set; }
		public string Comment { get; set; }
	}
}