namespace Web.Site.Api.Models
{
	public class ApiSubmitWithdraw
	{
		public decimal Amount { get; set; }
		public string Currency { get; set; }
		public int? CurrencyId { get; set; }
		public string Address { get; set; }
		public string PaymentId { get; set; }
	}
}