using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Deposit
{
	public class PeerInfoModel
	{
		public double? geobyteslatitude { get; set; }
		public double? geobyteslongitude { get; set; }
		public string Address { get; set; }
		public int StartingHeight { get; set; }
		public string AddNode
		{
			get { return $"addnode={Address}"; }
		}
	}
}
