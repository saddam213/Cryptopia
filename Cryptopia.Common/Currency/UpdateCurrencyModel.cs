using System.ComponentModel.DataAnnotations;
using Cryptopia.Enums;

namespace Cryptopia.Common.Currency
{
	public class UpdateCurrencyModel
	{
		public int Id { get; set; }
		public string Symbol { get; set; }
		public string Name { get; set; }

		[Range(typeof(decimal), "0", "10000000000")]
		public decimal PoolFee { get; set; }

		[Range(typeof(decimal), "0", "10000000000")]
		public decimal TradeFee { get; set; }

		[Range(typeof(decimal), "0", "10000000000")]
		public decimal WithdrawFee { get; set; }

		public WithdrawFeeType WithdrawFeeType { get; set; }

		[Range(typeof(decimal), "0.00000001", "10000000000")]
		public decimal WithdrawMin { get; set; }

		[Range(typeof(decimal), "0.00000001", "10000000000")]
		public decimal WithdrawMax { get; set; }

		[Range(typeof(decimal), "0", "10000000000")]
		public decimal TipMin { get; set; }

		[Range(typeof(decimal), "0", "10000000000")]
		public decimal MinBaseTrade { get; set; }

		[Range(1, 5000)]
		public int MinConfirmations { get; set; }

		public CurrencyStatus Status { get; set; }
		public string StatusMessage { get; set; }
		public CurrencyListingStatus ListingStatus { get; set; }
	}
}