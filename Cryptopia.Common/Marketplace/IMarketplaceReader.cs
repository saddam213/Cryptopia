using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptopia.Common.Marketplace
{
	public interface IMarketplaceReader
	{
		Task<List<MarketCategoryModel>> GetMarketCategories();
		Task<List<MarketListItemModel>> GetMarketItems(MarketItemSearchModel model);
		Task<MarketplaceItemModel> GetMarketItem(int marketItemId);
		Task<List<MarketplaceFeedbackModel>> GetUserFeedback(string username);
	}
}
