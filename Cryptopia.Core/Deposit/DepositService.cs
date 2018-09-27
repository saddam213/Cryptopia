using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Cryptopia.Common.Deposit;
using Cryptopia.Core.DepositService;
using Cryptopia.Base;
using System.Linq;
using Cryptopia.Enums;
using System.Text.RegularExpressions;

namespace Cryptopia.Core.Deposit
{
	public class DepositService : IDepositService
	{
		private static readonly string DepositServiceUsername = ConfigurationManager.AppSettings["DepositServiceUsername"];
		private static readonly string DepositServicePassword = ConfigurationManager.AppSettings["DepositServicePassword"];
		private static readonly string DepositServiceDomain = ConfigurationManager.AppSettings["DepositServiceDomain"];

		public async Task<bool> ValidateAddress(int currencyId, string address)
		{
			try
			{
				// If this is NZDT check if we have a valid bank account before asking Waves wallet
				if (currencyId == Constant.NZDT_ID)
				{
					if (Constant.NZDT_BANK_REGEX.IsMatch(address))
						return true;
				}

				using (var depositService = CreateService())
				{
					return await depositService.ValidateAddressAsync(currencyId, address).ConfigureAwait(false);
				}
			}
			catch (Exception)
			{
				return true;
			}
		}

		public async Task<List<string>> GenerateAddress(Guid userId, int currencyId)
		{
			try
			{
				using (var depositService = CreateService())
				{
					return await depositService.CreateAddressAsync(currencyId, userId).ConfigureAwait(false);
				}
			}
			catch (Exception)
			{
				return new List<string>();
			}
		}

		public async Task<List<PeerInfoModel>> GetPeerInfo(int currencyId)
		{
			try
			{
				using (var depositService = CreateService())
				{
					var data = await depositService.GetInfoAsync(currencyId).ConfigureAwait(false);
					if (data == null || data.PeerInfo.IsNullOrEmpty())
						return new List<PeerInfoModel>();

					return data.PeerInfo.Select(x => new PeerInfoModel
					{
						Address = x.addr,
						StartingHeight = x.startingheight
					}).ToList();

				}
			}
			catch (Exception)
			{
				return new List<PeerInfoModel>();
			}
		}

		public async Task<bool> Ping(int currencyId)
		{
			try
			{
				using (var depositService = CreateService())
				{
					var data = await depositService.GetInfoAsync(currencyId);
					if (data == null || data.InfoData == null)
						return true;

					return data.InfoData.blocks > 0;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public WalletInboundClient CreateService()
		{
			var client = new WalletInboundClient();
#if !DEBUG
			client.ClientCredentials.Windows.ClientCredential.UserName = DepositServiceUsername;
			client.ClientCredentials.Windows.ClientCredential.Password = DepositServicePassword;
			client.ClientCredentials.Windows.ClientCredential.Domain = DepositServiceDomain;
#endif
			return client;
		}
	}
}