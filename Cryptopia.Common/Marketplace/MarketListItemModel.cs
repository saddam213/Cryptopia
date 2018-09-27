using Cryptopia.Enums;

namespace Cryptopia.Common.Marketplace
{
	public class MarketListItemModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string MainImage { get; set; }
		public string Symbol { get; set; }
		public decimal Price { get; set; }
		public string Location { get; set; }
		public string Closes { get; set; }

		public MarketItemType ItemType { get; set; }
		public bool ReserveMet { get; set; }
		public bool NoReserve { get; set; }

		public MarketItemFeature Featured { get; set; }
		public bool IsAdult { get; set; }
	}
}
