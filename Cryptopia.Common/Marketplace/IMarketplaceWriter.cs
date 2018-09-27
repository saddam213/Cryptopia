using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.Marketplace
{
	public interface IMarketplaceWriter
	{
		Task<IWriterResult> CancelMarketItem(string userId, int marketItemId);
		Task<IWriterResult> CreateAnswer(string userId, CreateMarketAnswerModel model);
		Task<IWriterResult> CreateBid(string userId, CreateMarketBidModel model);
		Task<IWriterResult> CreateFeedback(string userId, CreateMarketFeedbackModel model);
		Task<IWriterResult<int>> CreateMarketItem(string userId, CreateMarketModel model);
		Task<IWriterResult> CreateQuestion(string userId, CreateMarketQuestionModel model);
		Task<IWriterResult> RelistMarketItem(string userId,int marketItemId);
		Task<IWriterResult> UpdateMarketItem(string userId, UpdateMarketModel model);
	}
}