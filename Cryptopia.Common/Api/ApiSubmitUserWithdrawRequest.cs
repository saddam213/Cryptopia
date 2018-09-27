using System;

namespace Cryptopia.Common.Api
{

	public class ApiSubmitUserWithdrawRequest
	{
		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>

		public Guid UserId { get; set; }

		/// <summary>
		/// Gets or sets the api request string.
		/// </summary>

		public string Request { get; set; }


		public int? CurrencyId { get; set; }


		public string Currency { get; set; }


		public decimal Amount { get; set; }


		public string Address { get; set; }
		public string PaymentId { get; set; }
	}
}