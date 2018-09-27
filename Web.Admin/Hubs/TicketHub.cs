namespace Web.Admin.Hubs
{
	using System.Threading.Tasks;
	using Microsoft.AspNet.SignalR;
	using System.Linq;
	using Cryptopia.Admin.Common.Support;

	[Authorize(Roles = "Admin")]
	public class TicketHub : Hub
	{
		private readonly static TicketConnectionMapping _connections = new TicketConnectionMapping();

		public async Task JoinTicketGroup(string ticketId)
		{
			string userName = Context.User.Identity.Name;
			_connections.AddConnection(userName, Context.ConnectionId, ticketId);
			await Groups.Add(Context.ConnectionId, ticketId);

			await Update();
		}

		public async Task UpdatingEditing(bool isEditing)
		{
			string userName = Context.User.Identity.Name;
			_connections.UpdateUserEditing(userName, Context.ConnectionId, isEditing);
			await Update();
		}

		public override async Task OnConnected()
		{
			await base.OnConnected();
		}

		public override async Task OnDisconnected(bool stopCalled)
		{
			string userName = Context.User.Identity.Name;
			_connections.RemoveConnection(userName, Context.ConnectionId);

			await Update();
			await base.OnDisconnected(stopCalled);
		}

		private async Task Update() {
			foreach (var item in _connections.GetConnectionState())
			{
				await Clients.Group(item.Key).Update(item.Value);
			}
			//await Clients.Clients(_connections.GetAllConnections()).Update(_connections.GetConnectionState());
		}

	}
}
