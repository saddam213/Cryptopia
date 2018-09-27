namespace Cryptopia.Admin.Common.AdminCurrency
{
	public class AdminCurrencyInfoModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Symbol { get; set; }
		public int Connections { get; set; }
		public string Status { get; set; }
		public string StatusMessage { get; set; }
		public string ListingStatus { get; set; }
		public string Version { get; set; }
		public string AlgoType { get; set; }
		public int Block { get; set; }
		public int Confirmations { get; set; }
		public string NetworkType { get; set; }
	}
}
