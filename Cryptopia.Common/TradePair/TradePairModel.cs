using Cryptopia.Enums;

namespace Cryptopia.Common.TradePair
{
	public class TradePairModel
	{
		public int TradePairId { get; set; }
		public string Market { get; set; }

		public int CurrencyId { get; set; }
		public string Symbol { get; set; }
		public string Name { get; set; }

		public int BaseCurrencyId { get; set; }
		public string BaseSymbol { get; set; }
		public string BaseName { get; set; }


		public decimal BaseFee { get; set; }
		public decimal BaseMinTrade { get; set; }

		public decimal LastTrade { get; set; }
		public double Change { get; set; }

		public TradePairStatus Status { get; set; }

		public string StatusMessage { get; set; }
	}
}
