namespace Cryptopia.Common.Marketplace
{
	public class MarketCategoryModel
	{
		public int Id { get; set; }
		public int? ParentId { get; set; }
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public int ItemCount { get; set; }
	}
}
