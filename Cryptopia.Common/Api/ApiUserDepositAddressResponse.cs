using System.Runtime.Serialization;

namespace Cryptopia.Common.Api
{
	[DataContract]
	public class ApiUserDepositAddressResponse
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

		[DataMember(Order = 2)]
		public ApiDepositAddressData Data { get; set; }
	}

	[DataContract]
	public class ApiDepositAddressData
	{
		[DataMember(Order = 0)]
		public string Currency { get; set; }

		[DataMember(Order = 1)]
		public string Address { get; set; }

		[DataMember(Order = 2)]
		public string BaseAddress { get; set; }
	}
}