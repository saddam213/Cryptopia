using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptopia.Common.Deposit
{
	public interface IDepositService
	{
		Task<bool> Ping(int currencyId);
		Task<bool> ValidateAddress(int currencyId, string address);
		Task<List<string>> GenerateAddress(Guid userId, int currencyId);
		Task<List<PeerInfoModel>> GetPeerInfo(int currencyId);
	}
}