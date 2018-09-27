using System.Runtime.Serialization;

namespace Cryptopia.Common.Api
{
	[DataContract]
	public class ApiCurrency
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		[DataMember(Order = 0)]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		[DataMember(Order = 1)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the symbol.
		/// </summary>
		[DataMember(Order = 2)]
		public string Symbol { get; set; }

		/// <summary>
		/// Gets or sets the algorithm.
		/// </summary>
		[DataMember(Order = 3)]
		public string Algorithm { get; set; }

		[DataMember(Order = 4)]
		public decimal WithdrawFee { get; set; }

		[DataMember(Order = 5)]
		public decimal MinWithdraw { get; set; }

		[DataMember(Order = 6)]
		public decimal MaxWithdraw { get; set; }

		[DataMember(Order = 7)]
		public decimal MinBaseTrade { get; set; }

		[DataMember(Order = 8)]
		public bool IsTipEnabled { get; set; }

		[DataMember(Order = 9)]
		public decimal MinTip { get; set; }

		[DataMember(Order = 10)]
		public int DepositConfirmations { get; set; }

		[DataMember(Order = 11)]
		public string Status { get; set; }

		[DataMember(Order = 12)]
		public string StatusMessage { get; set; }

		[DataMember(Order = 13)]
		public string ListingStatus { get; set; }
	}
}