using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Cache
{
	public class DistributedCache
	{
		private readonly IDatabase _rediscache;
		private readonly ConnectionMultiplexer _connection;
		private readonly NewtonsoftSerializer _serializer;
		private readonly string _lockToken = Guid.NewGuid().ToString();
		private readonly System.Runtime.Caching.MemoryCache _memorycache = new System.Runtime.Caching.MemoryCache("MemoryCache");
		private readonly bool _useDistributedCache;

		public DistributedCache(ConnectionMultiplexer connection, bool useRedis)
		{
			_useDistributedCache = useRedis;
			if (_useDistributedCache)
			{
				_connection = connection;
				_connection.PreserveAsyncOrder = false;
				_rediscache = _connection.GetDatabase();
			}
			_serializer = new NewtonsoftSerializer();
		}

		#region Implementation

		public async Task<T> GetOrSetMemoryAsync<T>(string key, TimeSpan expiry, Func<Task<T>> valueFactory)
		{
			return await GetOrSetMemory(key, expiry, valueFactory);
		}

		public async Task<T> GetOrSetDistributedAsync<T>(string key, TimeSpan expiry, Func<Task<T>> valueFactory) where T : class
		{
			try
			{
				if (!_useDistributedCache)
					return await GetOrSetMemory(key, expiry, valueFactory);

				var cacheData = await GetRedisData<T>(key);
				if (cacheData != null)
					return cacheData;

				var lockKey = $"lock_{key}";
				if (await _rediscache.LockTakeAsync(lockKey, _lockToken, TimeSpan.FromSeconds(60)))
				{
					try
					{
						var newValue = await valueFactory();
						if (newValue == null)
							return default(T);

						await SetRedis(key, expiry, newValue);
						return newValue;
					}
					finally
					{
						// Release the key lock;
						_rediscache.LockRelease(lockKey, _lockToken);
					}
				}
			}
			catch (Exception)
			{
			}
			return default(T);
		}



		public async Task<T> GetOrSetHybridAsync<T>(string key, TimeSpan expiry, Func<Task<T>> valueFactory) where T : class
		{
			try
			{
				if (!_useDistributedCache)
					return await GetOrSetMemory(key, expiry, valueFactory);

				try
				{
					// Get data from memory;
					var version = await GetRedisVersion(key);
					var cacheContainer = GetMemory<CacheContainer<T>>(key);

					// If we have memory data and the version is current, return data
					if (cacheContainer != null && cacheContainer.Data != null && cacheContainer.Version == version)
						return cacheContainer.Data;

					// If Version is out of date, get fresh data from cache
					if (version.HasValue)
					{
						var cacheData = await GetRedisData<T>(key);
						SetMemory(key, GetMemoryCaheTime(expiry), new CacheContainer<T>(version.Value, cacheData));
						return cacheData;
					}

					// The key does not exist, take a lock and get the data from valueFactory and populate mem and redis cache
					var lockKey = $"lock_{key}";
					if (await _rediscache.LockTakeAsync(lockKey, _lockToken, TimeSpan.FromSeconds(60)))
					{
						try
						{
							var newVersion = Guid.NewGuid();
							var newValue = await valueFactory();
							if (newValue == null)
								return default(T);

							await SetRedis(key, newVersion, expiry, newValue);
							SetMemory(key, GetMemoryCaheTime(expiry), new CacheContainer<T>(newVersion, newValue));
							return newValue;
						}
						finally
						{
							// release lock;
							_rediscache.LockRelease(lockKey, _lockToken);
						}
					}

					// The lock was held on the key, so return last memory data if we have it otherwise default(T)
					return cacheContainer?.Data;
				}
				catch (Exception)
				{

				}

				// redis down?, fallback to memcache
				return await GetOrSetMemoryAsync(key, expiry, valueFactory);
			}
			catch
			{
				return default(T);
			}
		}

		public async Task<T> GetOrSetHybridValueAsync<T>(string key, TimeSpan expiry, Func<Task<T>> valueFactory) where T : IConvertible
		{
			try
			{
				if (!_useDistributedCache)
					return await GetOrSetMemory(key, expiry, valueFactory);

				try
				{
					// Get data from memory;
					var version = await GetRedisVersion(key);
					var cacheContainer = GetMemory<CacheContainer<T>>(key);

					// If we have memory data and the version is current, return data
					if (cacheContainer != null && version == cacheContainer.Version)
						return cacheContainer.Data;

					// If Version is out of date, get fresh data from cache
					if (version.HasValue)
					{
						var cacheData = await GetRedisData<T>(key);
						SetMemory(key, GetMemoryCaheTime(expiry), new CacheContainer<T>(version.Value, cacheData));
						return cacheData;
					}

					// The key does not exist, take a lock and get the data from valueFactory and populate mem and redis cache
					var lockKey = $"lock_{key}";
					if (await _rediscache.LockTakeAsync(lockKey, _lockToken, TimeSpan.FromSeconds(60)))
					{
						try
						{
							var newVersion = Guid.NewGuid();
							var newValue = await valueFactory();
							if (newValue == null)
								return default(T);

							await SetRedis(key, newVersion, expiry, newValue);
							SetMemory(key, GetMemoryCaheTime(expiry), new CacheContainer<T>(newVersion, newValue));
							return newValue;
						}
						finally
						{
							// release lock;
							_rediscache.LockRelease(lockKey, _lockToken);
						}
					}

					// The lock was held on the key, so return last memory data if we have it otherwise default(T)
					return cacheContainer.Data;
				}
				catch (Exception)
				{

				}

				// redis down?, fallback to memcache
				return await GetOrSetMemory(key, expiry, valueFactory);
			}
			catch
			{
				return default(T);
			}
		}

		public async Task<List<T>> GetOrSetHybridList<T>(string key, TimeSpan expiry, Func<Task<List<T>>> valueFactory) where T : class
		{
			try
			{
				if (!_useDistributedCache)
					return await GetOrSetMemory(key, expiry, valueFactory);

				try
				{
					// Get data from memory;
					var version = await GetRedisVersion(key);
					var cacheContainer = GetMemory<CacheContainer<List<T>>>(key);

					// If we have memory data and the version is current, return data
					if (cacheContainer != null && version == cacheContainer.Version)
						return cacheContainer.Data;

					// If Version is out of date, get fresh data from cache
					if (version.HasValue)
					{
						var cacheData = await GetRedisList<T>(key);
						SetMemory(key, GetMemoryCaheTime(expiry), new CacheContainer<List<T>>(version.Value, cacheData));
						return cacheData;
					}

					// The key does not exist, take a lock and get the data from valueFactory and populate mem and redis cache
					var lockKey = $"lock_{key}";
					if (await _rediscache.LockTakeAsync(lockKey, _lockToken, TimeSpan.FromSeconds(60)))
					{
						try
						{
							var newVersion = Guid.NewGuid();
							var newValue = await valueFactory();
							if (newValue == null)
								return new List<T>();

							await SetRedisList(key, newVersion, expiry, newValue);
							SetMemory(key, GetMemoryCaheTime(expiry), new CacheContainer<List<T>>(newVersion, newValue));
							return newValue;
						}
						finally
						{
							// release lock;
							_rediscache.LockRelease(lockKey, _lockToken);
						}
					}

					// The lock was held on the key, so return last memory data if we have it otherwise default(T)
					return cacheContainer?.Data;
				}
				catch (Exception)
				{

				}

				// redis down?, fallback to memcache
				return await GetOrSetMemory(key, expiry, valueFactory);
			}
			catch
			{
				return new List<T>();
			}
		}

		public T GetListItem<T>(string listName, int index)
		{
			try
			{
				var memList = GetMemory<List<T>>(listName);
				if (!_useDistributedCache)
				{
					if (memList.Count > index)
						return memList[index];

					return default(T);
				}

				var redisData = _rediscache.HashGet(listName, index, flags: CommandFlags.PreferSlave);
				if (!redisData.HasValue)
					return default(T);

				var redisItem = _serializer.Deserialize<T>(redisData);

				// Update the item in memory
				if (memList.Count > index)
					memList[index] = redisItem;

				return redisItem;
			}
			catch (Exception)
			{
				return default(T);
			}
		}

		public void UpdateListItem<T>(string listName, int index, T listitem)
		{
			try
			{
				var redisItem = _serializer.Serialize(listitem);

				if (_useDistributedCache)
					_rediscache.HashSet(listName, new[] { new HashEntry(index, redisItem) });

				// Update the item in memory
				var memList = GetMemory<List<T>>(listName);
				if (memList.Count > index)
					memList[index] = listitem;
			}
			catch (Exception)
			{

			}
		}

		public void RemoveListItem<T>(string listName, int index)
		{
			try
			{
				var memList = GetMemory<List<T>>(listName);
				if (memList.Count > index)
					memList.RemoveAt(index);

				if (_useDistributedCache)
					_rediscache.HashDelete(listName, index);
			}
			catch (Exception)
			{

			}
		}

		public async Task<bool> AddIfNotExistsAsync(string key, string value, TimeSpan expiryTime)
		{
			try
			{
				if (!_useDistributedCache)
				{
					return System.Runtime.Caching.MemoryCache.Default.Add(key, value, DateTimeOffset.UtcNow.Add(expiryTime));
				}

				if (_rediscache == null)
					return false;

				return await _rediscache.StringSetAsync(key, value, expiry: expiryTime, when: When.NotExists);
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task InvalidateAsync(string key)
		{
			if (!_useDistributedCache)
			{
				InvalidateMemory(key);
				return;
			}

			await InvalidateRedis(key);
		}

		#endregion

		#region Redis

		private async Task SetRedis<T>(string key, TimeSpan expiry, T value)
		{
			if (_rediscache == null)
				return;

			try
			{
				var data = await _serializer.SerializeAsync(value);
				await _rediscache.HashSetAsync(key, "Data", data);
				await _rediscache.KeyExpireAsync(key, expiry);
			}
			catch (Exception)
			{
			}
		}

		private async Task SetRedis<T>(string key, Guid version, TimeSpan expiry, T value)
		{
			if (_rediscache == null)
				return;

			try
			{
				var data = await _serializer.SerializeAsync(value);
				var entries = new[]
				{
					new HashEntry("Data", data),
					new HashEntry("Version", version.ToString())
				};
				await _rediscache.HashSetAsync(key, entries);
				await _rediscache.KeyExpireAsync(key, expiry);
			}
			catch (Exception)
			{
			}
		}

		private async Task<Guid?> GetRedisVersion(string key)
		{
			if (_rediscache == null)
				return null;

			try
			{
				var version = await _rediscache.HashGetAsync(key, "Version", flags: CommandFlags.PreferSlave);
				if (!version.HasValue)
					return null;

				Guid output = Guid.Empty;
				if (Guid.TryParse(version, out output))
					return output;
			}
			catch (Exception)
			{
			}
			return null;
		}

		private async Task<T> GetRedisData<T>(string key)
		{
			if (_rediscache == null)
				return default(T);

			try
			{
				var version = await _rediscache.HashGetAsync(key, "Data", flags: CommandFlags.PreferSlave);
				if (!version.HasValue)
					return default(T);

				return await _serializer.DeserializeAsync<T>(version);
			}
			catch (Exception)
			{
			}
			return default(T);
		}

		private async Task<List<T>> GetRedisList<T>(string listName)
		{
			try
			{
				if (_rediscache == null)
					return new List<T>();

				var data = await _rediscache.HashGetAllAsync(listName, flags: CommandFlags.PreferSlave);
				if (data == null)
					return new List<T>();

				return data.Where(x => x.Name != "Version")
									.OrderBy(x => (int)x.Name)
									.Select(x => _serializer.Deserialize<T>(x.Value))
									.ToList();
			}
			catch (Exception)
			{
			}
			return new List<T>();
		}

		private async Task SetRedisList<T>(string listName, Guid version, TimeSpan expires, List<T> list)
		{
			try
			{
				if (_rediscache == null)
					return;

				var hashItems = new HashEntry[list.Count + 1];
				for (int i = 0; i < list.Count; i++)
				{
					hashItems[i] = new HashEntry(i, _serializer.Serialize(list[i]));
				}

				hashItems[list.Count + 1] = new HashEntry("Version", version.ToString());

				await _rediscache.KeyDeleteAsync(listName);
				await _rediscache.HashSetAsync(listName, hashItems);
				await _rediscache.KeyExpireAsync(listName, expires);
			}
			catch (Exception)
			{

			}
		}

		private async Task InvalidateRedis(string key)
		{
			try
			{
				if (_rediscache == null)
					return;

				await _rediscache.KeyDeleteAsync(key);
			}
			catch (Exception)
			{

			}
		}

		#endregion

		#region Memory

		private async Task<T> GetOrSetMemory<T>(string key, TimeSpan timespan, Func<Task<T>> valueFactory)
		{
			var newValue = new Lazy<Task<T>>(valueFactory);
			var oldValue = _memorycache.AddOrGetExisting(key, newValue, DateTimeOffset.UtcNow.Add(timespan)) as Lazy<Task<T>>;
			try
			{
				return await (oldValue ?? newValue).Value.ConfigureAwait(false);
			}
			catch
			{
				_memorycache.Remove(key);
				return default(T);
			}
		}

		private T SetMemory<T>(string key, TimeSpan timespan, T value)
		{
			_memorycache.Set(key, value, DateTimeOffset.UtcNow.Add(timespan));
			return value;
		}

		private T GetMemory<T>(string key) where T : class
		{
			return _memorycache.Get(key) as T;
		}

		private void InvalidateMemory(string key)
		{
			try
			{
				if (_memorycache == null)
					return;

				_memorycache.Remove(key);
			}
			catch (Exception)
			{
			}
		}

		private TimeSpan GetMemoryCaheTime(TimeSpan expiry)
		{
			if (expiry.TotalMinutes > 1)
				return expiry.Add(expiry);

			return TimeSpan.FromMinutes(2);
		}

		#endregion
	}
}
