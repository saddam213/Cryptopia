using System.ServiceModel;

namespace Cryptopia.MarketPlaceDataService
{
	[ServiceContract]
	public interface ICryptopiaMarketPlaceService
	{
		[OperationContract]
		string GetData(int value);
	}
}
