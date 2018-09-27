using System.Collections.Generic;
using System.Linq;

namespace Web.Admin.Hubs
{
	public class TicketConnectionMapping
	{
		private readonly Dictionary<string, HashSet<TicketConnectionMappingModel>> _connections;

		public TicketConnectionMapping()
		{
			_connections = new Dictionary<string, HashSet<TicketConnectionMappingModel>>();
		}

		public IList<string> GetAllConnections()
		{
			return _connections.Values.SelectMany(v => v.Select(x => x.ConnectionId)).ToList();
		}

		public IList<string> GetConnections(string userName)
		{
			HashSet<TicketConnectionMappingModel> connectionModels;
			if (!_connections.TryGetValue(userName, out connectionModels))
				return connectionModels.Select(m => m.ConnectionId).ToList();
			return new List<string>();
		}

		public Dictionary<string, IEnumerable<TicketUserStateModel>> GetConnectionState() {
			return _connections.SelectMany(connection => connection.Value.Select(model => new {
				model.TicketId,
				model.IsEditingTicket,
				UserName = connection.Key
			})).GroupBy(x => x.TicketId).ToDictionary(g => g.Key, g => g.Select(x => new TicketUserStateModel {
				UserName = x.UserName,
				IsEditingTicket = _connections[x.UserName].Where(m => m.TicketId == g.Key).Any(m => m.IsEditingTicket)
			}));
		}

		public void AddConnection(string userName, string connectionId, string ticketId)
		{
			lock (_connections)
			{
				HashSet<TicketConnectionMappingModel> connectionModels;
				if (!_connections.TryGetValue(userName, out connectionModels))
				{
					connectionModels = new HashSet<TicketConnectionMappingModel>();
					_connections.Add(userName, connectionModels);
				}
				lock (connectionModels)
				{
					connectionModels.Add(new TicketConnectionMappingModel(connectionId, ticketId));
				}
			}
		}

		public void UpdateUserEditing(string userName, string connectionId, bool isEditing)
		{
			lock (_connections)
			{
				HashSet<TicketConnectionMappingModel> connectionModels;
				if (!_connections.TryGetValue(userName, out connectionModels))
					return;

				lock (connectionModels)
				{
					var connectionModel = connectionModels.FirstOrDefault(m => m.ConnectionId == connectionId);
					connectionModel.IsEditingTicket = isEditing;
				}
			}
		}

		public void RemoveConnection(string userName, string connectionId)
		{
			lock (_connections)
			{
				HashSet<TicketConnectionMappingModel> connectionModels;
				if (!_connections.TryGetValue(userName, out connectionModels))
					return;
				lock (connectionModels)
				{
					connectionModels.RemoveWhere(m => m.ConnectionId == connectionId);
					if (connectionModels.Count == 0)
					{
						_connections.Remove(userName);
					}
				}
			}
		}
	}

	public class TicketConnectionMappingModel {

		public TicketConnectionMappingModel(string connectionId, string ticketId)
		{
			ConnectionId = connectionId;
			TicketId = ticketId;
		}

		public string ConnectionId { get; }
		public string TicketId { get; }

		public bool IsEditingTicket { get; set; }
	}

	public class TicketUserStateModel
	{
		public string UserName { get; set; }
		public bool IsEditingTicket { get; set; }
	}

}