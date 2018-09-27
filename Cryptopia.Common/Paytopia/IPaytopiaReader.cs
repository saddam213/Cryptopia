using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Paytopia
{
	public interface IPaytopiaReader
	{
		Task<PaytopiaItemModel> GetItem(PaytopiaItemType itemType);
		Task<List<PaytopiaItemModel>> GetItems();
		Task<List<TipSlotItemModel>> GetTipSlotItems();
		Task<List<FeaturedSlotItemModel>> GetFeaturedPoolSlotItems();
		Task<List<FeaturedSlotItemModel>> GetFeaturedCurrencySlotItems();
		Task<List<RewardSlotItemModel>> GetRewardSlotItems();
		Task<List<LottoSlotItemModel>> GetLottoSlotItems();
		Task<DataTablesResponse> GetPayments(string userId, DataTablesModel model);
		Task<List<PoolListingItemModel>> GetPoolListingItems();
		Task<List<string>> GetFlairItems();

		Task<PaytopiaPaymentModel> GetPayment(string userId, int id);
		Task<PaytopiaPaymentModel> AdminGetPayment(int id);
		Task<DataTablesResponse> AdminGetPayments(DataTablesModel model);
	}
}
