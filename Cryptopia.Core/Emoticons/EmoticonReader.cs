using Cryptopia.Common.Cache;
using Cryptopia.Common.Emoticons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Core.Emoticons
{
	public class EmoticonReader : IEmoticonReader
	{
		public ICacheService CacheService { get; set; }

		public async Task<EmoticonModel> AdminGetEmoticon(string emoticonFile, string code)
		{
			var emoticons = new List<EmoticonModel>();
			if (File.Exists(emoticonFile))
			{
				var serializer = new JavaScriptSerializer();
				using (var reader = new StreamReader(emoticonFile))
				{
					var data = await reader.ReadToEndAsync().ConfigureAwait(false);
					if (!string.IsNullOrEmpty(data))
					{
						emoticons = serializer.Deserialize<List<EmoticonModel>>(data);
					}
					return emoticons.FirstOrDefault(x => x.Code == code);
				}
			}
			return null;
		}

		public async Task<DataTablesResponse> AdminGetEmoticons(string emoticonFile, DataTablesModel model)
		{
			var emoticons = new List<EmoticonModel>();
			if (File.Exists(emoticonFile))
			{
				var serializer = new JavaScriptSerializer();
				using (var reader = new StreamReader(emoticonFile))
				{
					var data = await reader.ReadToEndAsync().ConfigureAwait(false);
					if (!string.IsNullOrEmpty(data))
					{
						emoticons = serializer.Deserialize<List<EmoticonModel>>(data);
					}
					return emoticons.GetDataTableResult(model);
				}
			}
			return model.GetEmptyDataTableResult();
		}

		public async Task<List<EmoticonModel>> GetEmoticons(string emoticonFile)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.Emoticons(), TimeSpan.FromMinutes(10), async () =>
			{
				var emoticons = new List<EmoticonModel>();
				if (File.Exists(emoticonFile))
				{
					var serializer = new JavaScriptSerializer();
					using (var reader = new StreamReader(emoticonFile))
					{
						var data = await reader.ReadToEndAsync().ConfigureAwait(false);
						if (!string.IsNullOrEmpty(data))
						{
							emoticons = serializer.Deserialize<List<EmoticonModel>>(data);
						}
					}
				}
				return emoticons;
			}).ConfigureAwait(false);
			return cacheResult;
		}
	}
}
