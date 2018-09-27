using System;

namespace Cryptopia.Common.Marketplace
{
	public class CreateMarketFeedbackModel
	{
		public string Comment { get; set; }
		public int MarketItemId { get; set; }
		public int Rating { get; set; }
		public Guid Receiver { get; set; }
	}
}
