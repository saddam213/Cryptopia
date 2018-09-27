using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Cache
{
	public class RedisConnectionFactory
	{
		private static readonly Lazy<ConnectionMultiplexer> WebCacheConnection;
		private static readonly Lazy<ConnectionMultiplexer> ChatCacheConnection;
		private static readonly Lazy<ConnectionMultiplexer> ApiKeyCacheConnection;
		private static readonly Lazy<ConnectionMultiplexer> ApiReplayCacheConnection;
		private static readonly Lazy<ConnectionMultiplexer> ApiThrottleCacheConnection;


		static RedisConnectionFactory()
		{
			var webCacheOptions = GetOptions(ConfigurationManager.AppSettings["RedisConnection_WebCache"]);
			WebCacheConnection = new Lazy<ConnectionMultiplexer>(() =>
				ConnectionMultiplexer.Connect(webCacheOptions)
			);

			var chatCacheOptions = GetOptions(ConfigurationManager.AppSettings["RedisConnection_ChatCache"]);
			ChatCacheConnection = new Lazy<ConnectionMultiplexer>(() =>
				ConnectionMultiplexer.Connect(chatCacheOptions)
			);

			var apiKeyCacheOptions = GetOptions(ConfigurationManager.AppSettings["RedisConnection_ApiKeyCache"]);
			ApiKeyCacheConnection = new Lazy<ConnectionMultiplexer>(() =>
				ConnectionMultiplexer.Connect(apiKeyCacheOptions)
			);

			var apiReplayCacheOptions = GetOptions(ConfigurationManager.AppSettings["RedisConnection_ApiReplayCache"]);
			ApiReplayCacheConnection = new Lazy<ConnectionMultiplexer>(() =>
				ConnectionMultiplexer.Connect(apiReplayCacheOptions)
			);

			var apiThrottleCacheOptions = GetOptions(ConfigurationManager.AppSettings["RedisConnection_ApiThrottleCache"]);
			ApiThrottleCacheConnection = new Lazy<ConnectionMultiplexer>(() =>
				ConnectionMultiplexer.Connect(apiThrottleCacheOptions)
			);
		}

		public static ConnectionMultiplexer GetWebCacheConnection() => WebCacheConnection.Value;
		public static ConnectionMultiplexer GetChatCacheConnection() => ChatCacheConnection.Value;
		public static ConnectionMultiplexer GetApiKeyCacheConnection() => ApiKeyCacheConnection.Value;
		public static ConnectionMultiplexer GetApiReplayCacheConnection() => ApiReplayCacheConnection.Value;
		public static ConnectionMultiplexer GetApiThrottleCacheConnection() => ApiThrottleCacheConnection.Value;

		private static ConfigurationOptions GetOptions(string connectionString)
		{
			var options = ConfigurationOptions.Parse(connectionString);
			options.AbortOnConnectFail = false;
			options.KeepAlive = 30;
			options.ConnectTimeout = 500;
			options.SyncTimeout = 500;
			options.ConnectRetry = 10;
			options.AllowAdmin = true;
			return options;
		}
	}
}
