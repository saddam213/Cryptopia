using System.Collections.Generic;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Common.Support
{
	public interface ISupportReader
	{
		Task<DataTablesResponse> GetOpenTickets(DataTablesModel model);
		Task<DataTablesResponse> GetClosedTickets(DataTablesModel model);

		Task<List<SupportTicketModel>> GetUserSupportTickets(string userId);
		Task<SupportTicketModel> GetUserSupportTicket(string userId, int ticketId);
	}
}