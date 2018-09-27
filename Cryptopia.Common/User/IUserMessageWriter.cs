using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.User
{
	public interface IUserMessageWriter
	{
		Task<IWriterResult> CreateMessage(string userId, UserMessageCreateModel model);
		Task<IWriterResult> DeleteMessage(string userId, int messageId);
		Task<IWriterResult> DeleteAllMessage(string userId, bool isInbox);
		Task<IWriterResult> ReportMessage(string userId, UserMessageReportModel model);
	}
}