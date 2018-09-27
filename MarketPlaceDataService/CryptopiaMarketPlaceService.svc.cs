namespace Cryptopia.MarketPlaceDataService
{
	public class CryptopiaMarketPlaceService : ICryptopiaMarketPlaceService
	{
		public string GetData(int value)
		{
			return string.Format("You entered: {0}", value);
		}
	}
}
