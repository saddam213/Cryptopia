using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.Paytopia
{
	public interface IPaytopiaWriter
	{
		Task<IWriterResult> UpdateTipSlot(string userId, TipSlotModel model);
		Task<IWriterResult> UpdateRewardSlot(string userId, RewardSlotModel model);
		Task<IWriterResult> UpdateLottoSlot(string userId, LottoSlotModel model);
		Task<IWriterResult> UpdateFeaturedPoolSlot(string userId, FeaturedSlotModel model);
		Task<IWriterResult> UpdateFeaturedCurrencySlot(string userId, FeaturedSlotModel model);
		Task<IWriterResult> UpdateExchangeListing(string userId, ExchangeListingModel model);
		Task<IWriterResult> UpdatePoolListing(string userId, PoolListingModel model);
		Task<IWriterResult> UpdateShares(string userId, SharesModel model);
		Task<IWriterResult> UpdateComboListing(string userId, ExchangeListingModel model);
		Task<IWriterResult> UpdateAvatar(string userId, AvatarModel model);
		Task<IWriterResult> UpdateEmoticon(string userId, EmoticonModel model);
		Task<IWriterResult> UpdateFlair(string userId, FlairModel model);
		Task<IWriterResult> UpdateTwoFactor(string userId, TwoFactorModel model);

		Task<IWriterResult> AdminUpdatePaytopiaPayment(AdminUpdatePaytopiaPaymentModel model);
	}
}
