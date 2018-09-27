using System;

namespace Cryptopia.Common.Api
{

	public class ApiUserDepositAddressRequest
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
	}
}