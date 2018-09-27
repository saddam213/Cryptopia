using Cryptopia.Enums;

namespace Web.Site.Api.Models
{
	public class ApiTransaction
	{
		public TransactionType Type { get; set; }
		public int? Count { get; set; }
	}
}