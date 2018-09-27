
using Cryptopia.Enums;
using System;

namespace Cryptopia.Common.Api
{

	public class ApiUserTransactionsRequest
	{
		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>

		public Guid UserId { get; set; }

		/// <summary>
		/// Gets or sets the api request string.
		/// </summary>

		public string Request { get; set; }


		public int? Count { get; set; }


		public TransactionType Type { get; set; }
	}


}