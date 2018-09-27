using System;

namespace Cryptopia.LottoService.Implementation
{
	public class PrizePayout
	{
		public Guid UserId { get; set; }
		public int CurrencyId { get; set; }
		public decimal Amount { get; set; }
	}
}
