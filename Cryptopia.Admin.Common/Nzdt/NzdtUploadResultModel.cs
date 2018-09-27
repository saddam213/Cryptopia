
namespace Cryptopia.Admin.Common.Nzdt
{
	public class NzdtUploadResultModel
	{
		public int TotalCount { get; set; }
		public int ExistingCount { get; set; }
		public int ReadyForProcessingCount { get; set; }
		public int ErroredCount { get; set; }
	}
}
