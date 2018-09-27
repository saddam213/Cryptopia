using System;

namespace Cryptopia.Common.Api
{
	public class ApiSubmitUserTipRequest
	{
		public Guid UserId { get; set; }
		public string Request { get; set; }
		public int? CurrencyId { get; set; }
		public string Currency { get; set; }
		public decimal Amount { get; set; }
		public int ActiveUsers { get; set; }
	}
}