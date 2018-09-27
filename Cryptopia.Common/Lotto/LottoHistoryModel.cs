using System;

namespace Cryptopia.Common.Lotto
{
	public class LottoHistoryModel
	{
		public int LottoItemId { get; set; }
		public string User { get; set; }
		public string Name { get; set; }
		public string Symbol { get; set; }
		public decimal Amount { get; set; }
		public decimal Percent { get; set; }
		public decimal Fee { get; set; }
		public decimal CharityFee { get; set; }
		public int LottoTicketId { get; set; }
		public int Position { get; set; }
		public int LottoDrawId { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
