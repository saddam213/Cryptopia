using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Cryptopia.Base.Logging;

namespace Cryptopia.PoolService
{
	[ServiceBehavior]
	public class PoolDataService : IPoolDataService
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(PoolDataService));

		public Task<string> DoSomething(int poolId)
		{
			throw new NotImplementedException();
		}
	}
}
