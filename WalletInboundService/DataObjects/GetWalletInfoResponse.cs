using Cryptopia.WalletAPI.DataObjects;
using System.Collections.Generic;

namespace Cryptopia.InboundService.DataObjects
{
	public class GetWalletInfoResponse
    {
        public GetInfoData InfoData { get; set; }
        public List<PeerInfo> PeerInfo { get; set; }
    }
}
