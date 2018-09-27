namespace Web.Admin.Hubs
{
	using System.Threading.Tasks;
	using Microsoft.AspNet.SignalR;

	public class SupportHub : Hub
	{
		[Authorize(Roles = "Admin")]
		public async Task TicketUpdated(int ticketId)
		{
			await Clients.All.TicketUpdated(ticketId);
		}

	}
}