using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Cache
{

	public class DistributedDictionary<T>
	{
		private readonly IDatabase _dataCache;
		private readonly ConnectionMultiplexer _connection;
		private readonly NewtonsoftSerializer _serializer;
		private readonly string _lockToken = Guid.NewGuid().ToString();
		private readonly bool _useRedisCache;
		private readonly string _dictionaryName = "Dictionary:";
		private readonly ConcurrentDictionary<string, T> _memoryCache = new ConcurrentDictionary<string, T>();

		public DistributedDictionary(ConnectionMultiplexer connection, string dictionaryName, bool useRedis)
		{
			_useRedisCache = useRedis;
			_dictionaryName = $"Dictionary:{dictionaryName}";
			if (_useRedisCache)
			{
				_connection = connection;
				_connection.PreserveAsyncOrder = false;
				_dataCache = _connection.GetDatabase();
			}
			_serializer = new NewtonsoftSerializer();
		}

		public async Task AddAsync(string key, T value)
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					_memoryCache.TryAdd(key, value);
					return;
				}

				var data = await _serializer.SerializeAsync(value);
				await _dataCache.HashSetAsync(_dictionaryName, key, data);
			}
			catch (Exception)
			{
			}
		}

		public async Task AddRangeAsync(Dictionary<string, T> values, bool clearFirst)
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					foreach (var value in values)
					{
						_memoryCache.TryAdd(value.Key, value.Value);
					}
					return;
				}

				var lockKey = $"lock_{_dictionaryName}";
				if (await _dataCache.LockTakeAsync(lockKey, _lockToken, TimeSpan.FromSeconds(60)))
				{
					try
					{
						if (clearFirst)
						{
							await ClearAsync();
						}

						var hashEntries = new List<HashEntry>();
						foreach (var value in values)
						{
							hashEntries.Add(new HashEntry(value.Key, await _serializer.SerializeAsync(value.Value)));
						}
						await _dataCache.HashSetAsync(_dictionaryName, hashEntries.ToArray());
					}
					finally
					{
						// Release the key lock;
						_dataCache.LockRelease(lockKey, _lockToken);
					}
				}

			}
			catch (Exception)
			{
			}
		}

		public async Task RemoveAsync(string key)
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					T data;
					_memoryCache.TryRemove(key, out data);
					return;
				}

				await _dataCache.HashDeleteAsync(_dictionaryName, key);
			}
			catch (Exception)
			{
			}
		}

		public async Task<Dictionary<string, T>> GetAsync()
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					return new Dictionary<string, T>(_memoryCache);
				}

				var data = await _dataCache.HashGetAllAsync(_dictionaryName, flags: CommandFlags.PreferSlave);
				if (data == null)
					return new Dictionary<string, T>();

				return data.ToDictionary(k => (string)k.Name, v => _serializer.Deserialize<T>(v.Value));
			}
			catch (Exception)
			{
				return new Dictionary<string, T>();
			}
		}

		public async Task<List<string>> GetKeysAsync()
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					return _memoryCache.Keys.ToList();
				}

				var data = await _dataCache.HashKeysAsync(_dictionaryName, flags: CommandFlags.PreferSlave);
				if (data == null)
					return new List<string>();

				return data.Select(k => (string)k).ToList();
			}
			catch (Exception)
			{
				return new List<string>();
			}
		}

		public async Task<List<T>> GetValuesAsync()
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					return _memoryCache.Values.ToList();
				}

				var data = await _dataCache.HashValuesAsync(_dictionaryName, flags: CommandFlags.PreferSlave);
				if (data == null)
					return new List<T>();

				return data.Select(k => _serializer.Deserialize<T>(k)).ToList();
			}
			catch (Exception)
			{
				return new List<T>();
			}
		}

		public async Task<T> GetValueAsync(string key)
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					T memdata;
					_memoryCache.TryGetValue(key, out memdata);
					return memdata;
				}


				var data = await _dataCache.HashGetAsync(_dictionaryName, key, flags: CommandFlags.PreferSlave);
				if (string.IsNullOrEmpty(data))
					return default(T);

				return await _serializer.DeserializeAsync<T>(data);
			}
			catch (Exception)
			{
				return default(T);
			}
		}

		public async Task<int> GetCountAsync()
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					return _memoryCache.Count;
				}

				return (int)await _dataCache.HashLengthAsync(_dictionaryName, flags: CommandFlags.PreferSlave);
			}
			catch (Exception)
			{
				return 0;
			}
		}

		public async Task ClearAsync()
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					_memoryCache.Clear();
				}

				await _dataCache.KeyDeleteAsync(_dictionaryName);
			}
			catch (Exception)
			{
			}
		}

		public async Task<bool> ExistsAsync(string key)
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					return _memoryCache.ContainsKey(key);
				}

				return await _dataCache.HashExistsAsync(_dictionaryName, key, flags: CommandFlags.PreferSlave);
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task AddRangeAsync(Func<Task<Dictionary<string, T>>> valueFactory, bool clearFirst)
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					var values = await valueFactory();
					foreach (var value in values)
					{
						_memoryCache.TryAdd(value.Key, value.Value);
					}
					return;
				}

				var lockKey = $"lock_{_dictionaryName}";
				if (await _dataCache.LockTakeAsync(lockKey, _lockToken, TimeSpan.FromSeconds(60)))
				{
					try
					{
						if (clearFirst)
						{
							await ClearAsync();
						}

						var values = await valueFactory();
						var hashEntries = new List<HashEntry>();
						foreach (var value in values)
						{
							hashEntries.Add(new HashEntry(value.Key, await _serializer.SerializeAsync(value.Value)));
						}
						await _dataCache.HashSetAsync(_dictionaryName, hashEntries.ToArray());
					}
					finally
					{
						// Release the key lock;
						_dataCache.LockRelease(lockKey, _lockToken);
					}
				}
			}
			catch (Exception)
			{
			}
		}

	}
}