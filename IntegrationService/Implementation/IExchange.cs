using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptopia.IntegrationService
{
	public interface IExchange
	{
		int Id { get; }
		string Name { get; }
		string Url { get; }
		Task<List<IExchangeMarket>> GetMarketData(Dictionary<string, int> tradePairMap);
	}
}
