using Cryptopia.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiThrottle;

namespace Web.Site.Cache
{
	public class RedisThrottleRepository : IThrottleRepository
	{
		private static DistributedDictionarySync<ThrottleCounterWrapper> _cache = new DistributedDictionarySync<ThrottleCounterWrapper>(RedisConnectionFactory.GetApiThrottleCacheConnection(), "ThrottleRepositoryKey", true);

		public bool Any(string id)
		{
			return _cache.Exists(id);
		}

		public ThrottleCounter? FirstOrDefault(string id)
		{
			var entry = _cache.GetValue(id);
			if (entry.Timestamp + entry.ExpirationTime < DateTime.UtcNow)
			{
				_cache.Remove(id);
				return null;
			}
			return new ThrottleCounter
			{
				Timestamp = entry.Timestamp,
				TotalRequests = entry.TotalRequests
			};
		}

		public void Save(string id, ThrottleCounter throttleCounter, TimeSpan expirationTime)
		{
			var entry = new ThrottleCounterWrapper
			{
				ExpirationTime = expirationTime,
				Timestamp = throttleCounter.Timestamp,
				TotalRequests = throttleCounter.TotalRequests
			};

			_cache.Add(id, entry);
		}

		public void Remove(string id)
		{
			_cache.Remove(id);
		}

		public void Clear()
		{
			_cache.Clear();
		}

		[Serializable]
		internal struct ThrottleCounterWrapper
		{
			public DateTime Timestamp { get; set; }

			public long TotalRequests { get; set; }

			public TimeSpan ExpirationTime { get; set; }
		}
	}
}
