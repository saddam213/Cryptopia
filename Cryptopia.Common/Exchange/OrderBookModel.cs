using System.Collections.Generic;

namespace Cryptopia.Common.Exchange
{
	public class OrderBookModel
	{
		public OrderBookModel()
		{
			BuyData = new List<string[]>();
			SellData = new List<string[]>();
		}
		public List<string[]> BuyData { get; set; }
		public List<string[]> SellData { get; set; }
	}
}
