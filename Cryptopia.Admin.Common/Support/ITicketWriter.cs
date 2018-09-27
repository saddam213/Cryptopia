namespace Cryptopia.Admin.Common.Support
{
	using System.Threading.Tasks;
	using Cryptopia.Enums;
	using Cryptopia.Infrastructure.Common.Results;

	public interface ITicketWriter
	{
		Task<IWriterResult> CreateTicket(CreateTicketModel model);
		Task<IWriterResult> UpdateTicket(string adminUserId, UpdateTicketModel model);
		Task<IWriterResult> MoveTicket(string adminUserId, int ticketId, int queueId);
		Task<IWriterResult> UpdateTicketStatus(string adminUserId, int ticketId, SupportTicketStatus status);
	}
}