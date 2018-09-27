using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

using AdmintopiaService.DataObjects;
using Cryptopia.WalletAPI.DataObjects;
using Cryptopia.Infrastructure.Incapsula.Common.Classes;
using Cryptopia.Infrastructure.Incapsula.Common.Enums;

namespace AdmintopiaService.Service
{
	[ServiceContract]
	public interface IAdmintopiaService
	{
        void Stop();

        [OperationContract]
		Task<List<WalletTransaction>> GetWalletTransactions(TransactionDataType transactionDataType, int currencyId, int walletTimeoutMinutes);

        [OperationContract]
        Task<List<WalletTransaction>> GetWalletTransactionsSince(TransactionDataType transactionDataType, int currencyId, int walletTimeoutMinutes, int searchBlockLength);

        [OperationContract]
        Task<IPBlacklist> BlacklistIpAddress(string ipAddress);

        [OperationContract]
        Task<IPBlacklist> GetIpAddressBlacklist();

        [OperationContract]
        Task<ResponseCode> PurgeSiteCache();
    }
}