using System;

namespace Cryptopia.Common.Lotto
{
	public class LottoTicketModel
	{
		public int LottoItemId { get; set; }
		public int LottoTicketId { get; set; }
		public int LottoDrawId { get; set; }
		public string Name { get; set; }
		public string Symbol { get; set; }
		public decimal Rate { get; set; }
		public DateTime NextDraw { get; set; }
	}
}
