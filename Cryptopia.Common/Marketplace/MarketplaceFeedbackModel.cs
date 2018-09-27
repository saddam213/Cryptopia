using System;

namespace Cryptopia.Common.Marketplace
{
	public class MarketplaceFeedbackModel
	{
		public int MarketItemId { get; set; }
		public double Rating { get; set; }
		public string Comment { get; set; }
		public string Sender { get; set; }
		public string Receiver { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
