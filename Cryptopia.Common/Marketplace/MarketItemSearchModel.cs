using Cryptopia.Enums;

namespace Cryptopia.Common.Marketplace
{
	public class MarketItemSearchModel
	{
		public int CategoryId { get; set; }
		public string SortBy { get; set; }
		public MarketItemType? ItemType { get; set; }
		public int LocationId { get; set; }
		public int CurrencyId { get; set; }
		public int Page { get; set; }
		public int ItemsPerPage { get; set; }
		public string SearchTerm { get; set; }
	}
}
