namespace Cryptopia.Admin.Common.Support
{
	using Cryptopia.Admin.Common.Reactopia;

	public class GetTicketsRequestModel
	{
		public bool IsClosed { get; set; }
		public int? QueueId { get; set; }
		public ReactDataTablesModel DataTablesModel { get; set; }
		public string TabSearch { get; set; }
	}
}
