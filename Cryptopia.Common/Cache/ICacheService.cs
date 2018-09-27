using System;
using System.Threading.Tasks;
using Cryptopia.Common.Mineshaft;

namespace Cryptopia.Common.Cache
{
	public interface ICacheService
	{
		Task<T> GetOrSetMemoryAsync<T>(string key, TimeSpan expiry, Func<Task<T>> valueFactory) where T : class;
		Task<T> GetOrSetHybridAsync<T>(string key, TimeSpan expiry, Func<Task<T>> valueFactory) where T : class;
		Task<T> GetOrSetMemoryValueAsync<T>(string key, TimeSpan expiry, Func<Task<T>> valueFactory) where T : IConvertible;
		Task<T> GetOrSetHybridValueAsync<T>(string key, TimeSpan expiry, Func<Task<T>> valueFactory) where T : IConvertible;
		Task InvalidateAsync(params string[] keys);
	}
}
