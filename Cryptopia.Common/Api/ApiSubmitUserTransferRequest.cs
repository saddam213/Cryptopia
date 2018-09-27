using Cryptopia.Enums;
using System;

namespace Cryptopia.Common.Api
{

	public class ApiSubmitUserTransferRequest
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


		public string UserName { get; set; }


		public decimal Amount { get; set; }


		public TransferType Type { get; set; }
	}
}