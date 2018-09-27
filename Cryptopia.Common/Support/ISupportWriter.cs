using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.Support
{
	public interface ISupportWriter
	{
		Task<IWriterResult> CreateTicket(string userId, CreateSupportTicketModel model);
		Task<IWriterResult> UpdateTicketStatus(string userId, UpdateSupportTicketModel model);
		Task<IWriterResult> CreateTicketReply(string userId, CreateSupportReplyModel model);
	}
}
