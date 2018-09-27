using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Cache
{
	public class NewtonsoftSerializer 
	{
		private static readonly Encoding encoding = Encoding.UTF8;

		private readonly JsonSerializerSettings settings;

		public NewtonsoftSerializer(JsonSerializerSettings settings = null)
		{
			this.settings = settings ?? new JsonSerializerSettings();
		}

		public byte[] Serialize(object item)
		{
			var type = item.GetType();
			var jsonString = JsonConvert.SerializeObject(item, type, settings);
			return encoding.GetBytes(jsonString);
		}

		public async Task<byte[]> SerializeAsync(object item)
		{
			var type = item.GetType();
			var jsonString = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(item, type, settings));
			return encoding.GetBytes(jsonString);
		}

		public object Deserialize(byte[] serializedObject)
		{
			var jsonString = encoding.GetString(serializedObject);
			return JsonConvert.DeserializeObject(jsonString, typeof(object));
		}

		public Task<object> DeserializeAsync(byte[] serializedObject)
		{
			return Task.Factory.StartNew(() => Deserialize(serializedObject));
		}

		public T Deserialize<T>(byte[] serializedObject)
		{
			var jsonString = encoding.GetString(serializedObject);
			return JsonConvert.DeserializeObject<T>(jsonString, settings);
		}

		public Task<T> DeserializeAsync<T>(byte[] serializedObject)
		{
			return Task.Factory.StartNew(() => Deserialize<T>(serializedObject));
		}
	}
}
