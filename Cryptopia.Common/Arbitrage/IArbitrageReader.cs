using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptopia.Common.Arbitrage
{
	public interface IArbitrageReader
	{
		Task<List<ArbitrageDataModel>> GetData();
	}
}
