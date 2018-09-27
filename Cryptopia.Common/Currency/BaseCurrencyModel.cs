namespace Cryptopia.Common.Currency
{
	public class BaseCurrencyModel
	{
		public string Name { get; set; }
		public string Symbol { get; set; }
		public int CurrencyId { get; set; }
		public int DefaultTradePairId { get; set; }
		public int Rank { get; set; }
	}
}