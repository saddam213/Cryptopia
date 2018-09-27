using Cryptopia.Common.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Mineshaft
{
	public class MinersModel
	{
		public MinersModel()
		{
			Connections = new List<PoolConnectionModel>();
		}
		public List<PoolConnectionModel> Connections { get; set; }
	}
}
