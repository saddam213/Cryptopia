﻿using System.Runtime.Serialization;

namespace Cryptopia.Common.Api
{
	[DataContract]
	public class ApiMarketOrdersResult
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ApiMarketOrdersResult"/> is success.
		/// </summary>
		/// <value>
		///   <c>true</c> if success; otherwise, <c>false</c>.
		/// </value>
		[DataMember(Order = 0)]
		public bool Success { get; set; }

		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		[DataMember(Order = 1)]
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		[DataMember(Order = 2)]
		public ApiMarketOrderData Data { get; set; }

		[DataMember(Order = 3)]
		public string Error { get; set; }
	}
}