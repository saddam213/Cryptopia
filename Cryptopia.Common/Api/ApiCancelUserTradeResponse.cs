﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Cryptopia.Common.Trade;

namespace Cryptopia.Common.Api
{
	[DataContract]
	public class ApiCancelUserTradeResponse
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ApiCurrencyResult"/> is success.
		/// </summary>
		/// <value>
		///   <c>true</c> if success; otherwise, <c>false</c>.
		/// </value>
		[DataMember(Order = 0)]
		public bool Success { get; set; }

		[DataMember(Order = 1)]
		public string Error { get; set; }

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		[DataMember(Order = 2)]
		public List<int> Data { get; set; }
	}
}