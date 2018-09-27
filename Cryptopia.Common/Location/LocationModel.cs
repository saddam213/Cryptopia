namespace Cryptopia.Common.Location
{
	public class LocationModel
	{
		public int Id { get; set; }
		public int? ParentId { get; set; }
		public string Name { get; set; }
		public string CountryCode { get; set; }
	}
}
