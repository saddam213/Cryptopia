namespace Cryptopia.Common.Exchange
{
	public class ExchangeSummaryBaseCurrency
	{
		public string Name { get; set; }
		public string Symbol { get; set; }
		public int CurrencyId { get; set; }
		public decimal TotalVolume { get; set; }
		public int Rank { get; set; }
	}
}
