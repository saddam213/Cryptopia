namespace Web.Site.Api.Models
{
	public class ApiSubmitTip
	{
		public decimal Amount { get; set; }
		public string Currency { get; set; }
		public int? CurrencyId { get; set; }
		public int ActiveUsers { get; set; }
	}
}