using System;

namespace Cryptopia.Common.Marketplace
{
	public class MarketQuestionModel
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Question { get; set; }
		public string Answer { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
