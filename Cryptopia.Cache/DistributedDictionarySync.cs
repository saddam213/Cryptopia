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

	public class DistributedDictionarySync<T>
	{
		private readonly IDatabase _dataCache;
		private readonly ConnectionMultiplexer _connection;
		private readonly NewtonsoftSerializer _serializer;
		private readonly string _lockToken = Guid.NewGuid().ToString();
		private readonly bool _useRedisCache;
		private readonly string _dictionaryName = "Dictionary:";
		private readonly ConcurrentDictionary<string, T> _memoryCache = new ConcurrentDictionary<string, T>();

		public DistributedDictionarySync(ConnectionMultiplexer connection, string dictionaryName, bool useRedis)
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

		public void Add(string key, T value)
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					_memoryCache.TryAdd(key, value);
					return;
				}

				var data = _serializer.Serialize(value);
				_dataCache.HashSet(_dictionaryName, key, data);
			}
			catch (Exception)
			{
			}
		}

		public void AddRange(Dictionary<string, T> values)
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

				var hashEntries = new List<HashEntry>();
				foreach (var value in values)
				{
					hashEntries.Add(new HashEntry(value.Key, _serializer.Serialize(value.Value)));
				}
				_dataCache.HashSet(_dictionaryName, hashEntries.ToArray());
			}
			catch (Exception)
			{
			}
		}

		public void Remove(string key)
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					T data;
					_memoryCache.TryRemove(key, out data);
					return;
				}

				_dataCache.HashDelete(_dictionaryName, key);
			}
			catch (Exception)
			{
			}
		}

		public Dictionary<string, T> Get()
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					return new Dictionary<string, T>(_memoryCache);
				}

				var data = _dataCache.HashGetAll(_dictionaryName, flags: CommandFlags.PreferSlave);
				if (data == null)
					return new Dictionary<string, T>();

				return data.ToDictionary(k => (string)k.Name, v => _serializer.Deserialize<T>(v.Value));
			}
			catch (Exception)
			{
				return new Dictionary<string, T>();
			}
		}

		public List<string> GetKeys()
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					return _memoryCache.Keys.ToList();
				}

				var data = _dataCache.HashKeys(_dictionaryName, flags: CommandFlags.PreferSlave);
				if (data == null)
					return new List<string>();

				return data.Select(k => (string)k).ToList();
			}
			catch (Exception)
			{
				return new List<string>();
			}
		}

		public List<T> GetValues()
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					return _memoryCache.Values.ToList();
				}

				var data = _dataCache.HashValues(_dictionaryName, flags: CommandFlags.PreferSlave);
				if (data == null)
					return new List<T>();

				return data.Select(k => _serializer.Deserialize<T>(k)).ToList();
			}
			catch (Exception)
			{
				return new List<T>();
			}
		}

		public T GetValue(string key)
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					T memdata;
					_memoryCache.TryGetValue(key, out memdata);
					return memdata;
				}


				var data = _dataCache.HashGet(_dictionaryName, key, flags: CommandFlags.PreferSlave);
				if (string.IsNullOrEmpty(data))
					return default(T);

				return _serializer.Deserialize<T>(data);
			}
			catch (Exception)
			{
				return default(T);
			}
		}

		public int GetCount()
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					return _memoryCache.Count;
				}

				return (int)_dataCache.HashLength(_dictionaryName, flags: CommandFlags.PreferSlave);
			}
			catch (Exception)
			{
				return 0;
			}
		}

		public void Clear()
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					_memoryCache.Clear();
				}

				_dataCache.KeyDelete(_dictionaryName);
			}
			catch (Exception)
			{
			}
		}

		public bool Exists(string key)
		{
			try
			{
				if (_useRedisCache == false || _dataCache == null)
				{
					return _memoryCache.ContainsKey(key);
				}

				return _dataCache.HashExists(_dictionaryName, key, flags: CommandFlags.PreferSlave);
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}