using System;

namespace Cryptopia.Common.Marketplace
{
	public class CreateMarketQuestionModel
	{
		public int MarketItemId { get; set; }
		public Guid MarketItemUserId { get; set; }
		public string Question { get; set; }
	}
}
