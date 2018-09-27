using System.Collections.Generic;
using Cryptopia.Enums;

namespace Cryptopia.Admin.Common.Support
{
	using System.Threading.Tasks;
	using Cryptopia.Infrastructure.Common.DataTables;

	public interface ISupportReader
	{
		Task<DataTablesResponse> GetTickets(GetTicketsRequestModel model);
		Task<TicketDetailsViewModel> GetTicket(int id);
		Task<List<SupportQueueModel>> GetSupportQueues();
		Task<UpdateTicketModel> GetUpdateTicket(int id);
		Task<CreateTicketModel> GetCreateTicket();
	}
}