interface SignalR {
	supportHub: SupportHubProxy
}

interface SupportHubProxy {
	client: SupportHubClient;
}
interface SupportHubClient {
	TicketUpdated: (ticketId: number) => void;
}
