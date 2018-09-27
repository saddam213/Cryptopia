using Cryptopia.Cache;
using Cryptopia.Common.Cache;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Cryptopia.Core.Cache
{
	public class CacheService : ICacheService
	{
		private static bool _useRedisCache = bool.Parse(ConfigurationManager.AppSettings["Redis_WebCache_Enabled"]);
		private static DistributedCache _distributedCache = new DistributedCache(RedisConnectionFactory.GetWebCacheConnection(), _useRedisCache);

		public async Task<T> GetOrSetMemoryAsync<T>(string key, TimeSpan expiry, Func<Task<T>> valueFactory) where T : class
		{
			return await _distributedCache.GetOrSetMemoryAsync<T>(key, expiry, valueFactory);
		}

		public async Task<T> GetOrSetHybridAsync<T>(string key, TimeSpan expiry, Func<Task<T>> valueFactory) where T : class
		{
			return await _distributedCache.GetOrSetHybridAsync<T>(key, expiry, valueFactory);
		}

		public async Task<T> GetOrSetMemoryValueAsync<T>(string key, TimeSpan expiry, Func<Task<T>> valueFactory) where T : IConvertible
		{
			return await _distributedCache.GetOrSetMemoryAsync<T>(key, expiry, valueFactory);
		}

		public async Task<T> GetOrSetHybridValueAsync<T>(string key, TimeSpan expiry, Func<Task<T>> valueFactory) where T : IConvertible
		{
			return await _distributedCache.GetOrSetHybridValueAsync<T>(key, expiry, valueFactory);
		}

		public async Task InvalidateAsync(params string[] keys)
		{
			if (keys == null)
				return;

			foreach (var key in keys)
			{
				await _distributedCache.InvalidateAsync(key);
			}
		}

	}
}