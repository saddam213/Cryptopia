using Cryptopia.Enums;

namespace Cryptopia.Common.Trade
{
	public class CancelTradeModel
	{
		public CancelTradeType CancelType { get; set; }
		public int TradeId { get; set; }
		public int TradePairId { get; set; }
	}
}
