using Cryptopia.Enums;

namespace Cryptopia.Admin.Common.Support
{
	public class SupportQueueModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Order { get; set; }

		public bool HasOpenTickets { get; set; }

		public bool IsDefaultQueue => this.Id == Constant.DEFAULT_QUEUE_ID;
	}
}