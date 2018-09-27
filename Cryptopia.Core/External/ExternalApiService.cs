using Cryptopia.Common.Cache;
using Cryptopia.Common.External;
using Cryptopia.Enums;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Cryptopia.Core.External
{
	public class ExternalApiService : IExternalApiService
	{
		public ICacheService CacheService { get; set; }

		public async Task<decimal> ConvertDollarToBTC(decimal dollarAmount, ConvertDollarToBTCType dollarType)
		{
			try
			{
				var cacheResult = await CacheService.GetOrSetHybridValueAsync(CacheKey.ConvertDollarToBTC(dollarAmount, dollarType), TimeSpan.FromMinutes(120), async () =>
				{
					using (var client = new WebClient())
					{
						decimal amount = 0m;
						var uri = new Uri($"https://blockchain.info/tobtc?currency={dollarType}&value={dollarAmount}");
						var result = await client.DownloadStringTaskAsync(uri).ConfigureAwait(false);
						if (string.IsNullOrEmpty(result))
							return amount;

						decimal.TryParse(result, out amount);
						return amount;
					}
				}).ConfigureAwait(false);

				if(cacheResult == 0m)
					await CacheService.InvalidateAsync(CacheKey.ConvertDollarToBTC(dollarAmount, dollarType));

				return cacheResult;
			}
			catch
			{
				
			}
			await CacheService.InvalidateAsync(CacheKey.ConvertDollarToBTC(dollarAmount, dollarType));
			return 0m;
		}

	}
}
