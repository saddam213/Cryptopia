using System.Collections.Generic;

namespace Cryptopia.Common.Exchange
{
	public class UserOrderModel
	{
		public UserOrderModel()
		{
			OrderData = new List<string[]>();
			HistoryData = new List<string[]>();
		}
		public List<string[]> HistoryData { get; set; }
		public List<string[]> OrderData { get; set; }
	}
}
