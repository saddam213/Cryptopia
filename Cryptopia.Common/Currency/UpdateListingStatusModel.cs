using System;
using Cryptopia.Enums;

namespace Cryptopia.Common.Currency
{
	public class UpdateListingStatusModel
	{
		public int CurrencyId { get; set; }
		public string Symbol { get; set; }
		public string Name { get; set; }

		public CurrencyStatus Status { get; set; }
		public string StatusMessage { get; set; }
		public CurrencyListingStatus ListingStatus { get; set; }
		public DateTime? DelistOn { get; set; }
		public string TwoFactorCode { get; set; }
	}
}