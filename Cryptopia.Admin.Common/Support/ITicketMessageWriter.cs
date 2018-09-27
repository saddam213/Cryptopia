namespace Cryptopia.Admin.Common.Support
{
	using System.Threading.Tasks;
	using Cryptopia.Infrastructure.Common.Results;

	public interface ITicketMessageWriter
	{
		Task<IWriterResult> CreateMessage(int ticketId, string adminUserId, string message);
		Task<IWriterResult> CreateAdminMessage(int ticketId, string adminUserId, string message);
		Task<IWriterResult> DeleteMessage(int messageId, string adminUserId);
		Task<IWriterResult<PublishReplyResultModel>> PublishMessage(int messageId, string adminUserId);
		Task<IWriterResult> EditMessage(int messageId, string adminUserId, string message);

	}
}