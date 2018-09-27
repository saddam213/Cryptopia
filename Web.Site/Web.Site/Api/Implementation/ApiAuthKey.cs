using Cryptopia.Cache;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.User;
using Cryptopia.Data.DataContext;
using Cryptopia.Infrastructure.Common.DataContext;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Site.Api.Authentication
{
	public class ApiKeyStore
	{
		private static bool _useRedisCache = bool.Parse(ConfigurationManager.AppSettings["Redis_ApiKeyCache_Enabled"]);
		private static DistributedDictionary<UserApiAuthKey> _rediscache = new DistributedDictionary<UserApiAuthKey>(RedisConnectionFactory.GetApiKeyCacheConnection(), "ApiKeyList", _useRedisCache);

		public static async Task<bool> UpdateApiAuthKey(UpdateApiModel apiData)
		{
			await _rediscache.RemoveAsync(apiData.OldApiKey);
			var result = await GetApiAuthKey(apiData.ApiKey);
			return result != null;
		}

		public static async Task<UserApiAuthKey> GetApiAuthKey(string apiKey)
		{
			var existing = await _rediscache.GetValueAsync(apiKey);
			if (existing != null)
				return existing;

			using (var context = new ApplicationDbContext())
			{
				existing = await context.Users
				.Where(x => x.ApiKey == apiKey)
				.Select(x => new UserApiAuthKey
				{
					UserId = x.Id,
					Key = x.ApiKey,
					Secret = x.ApiSecret,
					IsEnabled = !x.IsDisabled && x.IsApiEnabled
				}).FirstOrDefaultNoLockAsync();
			}

			if (existing != null)
				await _rediscache.AddAsync(apiKey, existing);

			return existing;
		}
	}
}
