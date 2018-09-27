using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Base.Serialization
{
	public static class BinarySerializer
	{
		public static byte[] Serialize(object o)
		{
			if (o == null)
			{
				return null;
			}

			var binaryFormatter = new BinaryFormatter();
			using (var memoryStream = new MemoryStream())
			{
				binaryFormatter.Serialize(memoryStream, o);
				var objectDataAsStream = memoryStream.ToArray();
				return objectDataAsStream;
			}
		}

		public static T Deserialize<T>(byte[] stream)
		{
			if (stream == null)
			{
				return default(T);
			}

			var binaryFormatter = new BinaryFormatter();
			using (var memoryStream = new MemoryStream(stream))
			{
				var result = (T)binaryFormatter.Deserialize(memoryStream);
				return result;
			}
		}
	}
}
