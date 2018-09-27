using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Exchange
{
	public class OrderData
	{
		public decimal Amount { get; set; }
		public decimal Rate { get; set; }
		public string Symbol { get; set; }
		public decimal Total { get; set; }
		public TradeHistoryType Type { get; set; }
	}

	public class TradeHistoryData
	{
		public decimal Amount { get; set; }
		public decimal Rate { get; set; }
		public DateTime Timestamp { get; set; }
		public int TradePairId { get; set; }
	}

	public class TradeSummaryData
	{
		public int CurrencyId { get; set; }
		public int TradePairId { get; set; }
		public string Name { get; set; }
		public string Symbol { get; set; }
		public string Change { get; set; }
		public string BaseVolume { get; set; }
		public string Volume { get; set; }
		public string High { get; set; }
		public string Low { get; set; }
		public string Last { get; set; }
		public string BaseSymbol { get; set; }
	}
}
