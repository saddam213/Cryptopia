namespace Web.Site.Api.Models
{
	public class ApiSubmitTransfer
	{
		public decimal Amount { get; set; }
		public string Currency { get; set; }
		public int? CurrencyId { get; set; }
		public string UserName { get; set; }
	}
}