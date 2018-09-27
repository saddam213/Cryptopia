using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Cryptopia.PoolService
{
	[ServiceContract]
	public interface IPoolDataService
	{
		[OperationContract]
		Task<string> DoSomething(int poolId);
	}
}
