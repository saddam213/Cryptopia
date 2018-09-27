using Cryptopia.Common.Deposit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Currency
{
	public class CurrencyPeerInfoModel
	{
		public string Symbol { get; set; }
		public string Name { get; set; }
		public List<PeerInfoModel> PeerInfo { get; set; }

		public string AddNodes
		{
			get
			{
				if(PeerInfo != null && PeerInfo.Any())
				{
					return string.Join(Environment.NewLine, PeerInfo.Select(x => x.AddNode));
				}
				return "No active nodes found.";
			}
		}
	}
}
