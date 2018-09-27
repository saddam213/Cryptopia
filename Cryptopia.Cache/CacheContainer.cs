using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Cache
{
	public class CacheContainer<T>
	{
		public CacheContainer()
		{

		}
		public CacheContainer(Guid version, T data)
		{
			Data = data;
			Version = version;
		}
		public Guid Version { get; set; }
		public T Data { get; set; }
	}
}
