using Cryptopia.Base;
using Cryptopia.Common.Cache;
using Cryptopia.Common.Chat;
using Cryptopia.Common.Emoticons;
using Cryptopia.Common.Image;
using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.Emoticons
{
	public class EmoticonWriter : IEmoticonWriter
	{
		public IImageService ImageService { get; set; }
		public ICacheService CacheService { get; set; }

		public async Task<IWriterResult> CreateEmoticon(string emoticonFile, CreateEmoticonModel model)
		{
			if (!File.Exists(emoticonFile))
				return new WriterResult(false, "Emoticon file not found.");

			var data = File.ReadAllText(emoticonFile);
			if (string.IsNullOrEmpty(data))
				return new WriterResult(false, "Emoticon file not found.");

			var serializer = new JavaScriptSerializer();
			var emoticons = new List<EmoticonModel>(serializer.Deserialize<List<EmoticonModel>>(data));
			var code = !string.IsNullOrEmpty(model.Code) ? model.Code : GetNewCode(emoticons.Select(x => x.Code));
			if (string.IsNullOrEmpty(code))
				return new WriterResult(false, "Failed to generate emoticon code.");

			if(emoticons.Any(x => x.Code == code))
				return new WriterResult(false, $"Code '{code}' already exists.");

			var extension = model.ForceResize ? ".PNG" : ".GIF";
			var imageResult = await ImageService.SaveImage(new CreateImageModel
			{
				CanResize = model.ForceResize,
				PreserveAspectRatio = model.ForceResize,
				Directory = Path.GetDirectoryName(emoticonFile),
				FileStream = model.FileStream,
				MaxFileSize = Constant.EMOTICON_MAXSIZE,
				MaxHeight = Constant.EMOTICON_MAXHEIGHT,
				MaxWidth = Constant.EMOTICON_MAXWIDTH,
				Name = code,
				Extention = extension
			}).ConfigureAwait(false);
			if (!imageResult.Success)
				return new WriterResult(false, imageResult.Message);

			emoticons.Add(new EmoticonModel
			{
				Category = model.Category,
				Code = $"[{code}]",
				Name = model.Name,
				Path = code + extension
			});
			File.WriteAllText(emoticonFile, serializer.Serialize(emoticons.OrderBy(x => x.Code).ToList()));
			await CacheService.InvalidateAsync(CacheKey.Emoticons()).ConfigureAwait(false);

			model.Code = $"[{code}]";
			return new WriterResult(true, "Successfully saved emoticon.");
		}

		public async Task<IWriterResult> AdminUpdateEmoticon(string emoticonFile, UpdateEmoticonModel model)
		{
			if (!File.Exists(emoticonFile))
				return new WriterResult(false, "Emoticon file not found.");

			var data = File.ReadAllText(emoticonFile);
			if (string.IsNullOrEmpty(data))
				return new WriterResult(false, "Emoticon file not found.");

			var serializer = new JavaScriptSerializer();
			var emoticons = new List<EmoticonModel>(serializer.Deserialize<List<EmoticonModel>>(data));
			var emoticon = emoticons.FirstOrDefault(x => x.Code == model.Code);
			if(emoticon == null)
				return new WriterResult(false, $"Emoticon '{model.Code}' not found.");

			emoticon.Category = model.Category;
			emoticon.Name = model.Name;
		
			File.WriteAllText(emoticonFile, serializer.Serialize(emoticons));
			await CacheService.InvalidateAsync(CacheKey.Emoticons()).ConfigureAwait(false);
			return new WriterResult(true, "Successfully saved emoticon.");
		}

		public async Task<IWriterResult> AdminDeleteEmoticon(string emoticonFile, DeleteEmoticonModel model)
		{
			if (!File.Exists(emoticonFile))
				return new WriterResult(false, "Emoticon file not found.");

			var data = File.ReadAllText(emoticonFile);
			if (string.IsNullOrEmpty(data))
				return new WriterResult(false, "Emoticon file not found.");

			var serializer = new JavaScriptSerializer();
			var emoticons = new List<EmoticonModel>(serializer.Deserialize<List<EmoticonModel>>(data));
			var emoticon = emoticons.FirstOrDefault(x => x.Code == model.Code);
			if (emoticon == null)
				return new WriterResult(false, $"Emoticon '{model.Code}' not found.");

			emoticons.Remove(emoticon);

			File.WriteAllText(emoticonFile, serializer.Serialize(emoticons));
			await CacheService.InvalidateAsync(CacheKey.Emoticons()).ConfigureAwait(false);
			return new WriterResult(true, "Successfully deleted emoticon.");
		}

		private static Random _random = new Random();
		private static string GenerateCode()
		{
			var letterCount = 2;
			var numberCount = 1;
			const string nums = "0123456789";
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			var letters = new string(Enumerable.Repeat(chars, letterCount).Select(s => s[_random.Next(s.Length)]).ToArray());
			var numbers = new string(Enumerable.Repeat(nums, numberCount).Select(s => s[_random.Next(s.Length)]).ToArray());
			return letters + numbers;
		}

		private static string GetNewCode(IEnumerable<string> existingCodes)
		{
			if (existingCodes.IsNullOrEmpty())
				return GenerateCode();

			foreach (var existingCode in existingCodes)
			{
				var code = GenerateCode();
				if (!existingCodes.Contains($"[{code}]"))
					return code;
			}
			return string.Empty;
		}
	}
}
