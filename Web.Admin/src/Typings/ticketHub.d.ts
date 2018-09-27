interface SignalR {
	ticketHub: TicketHubProxy
}

interface TicketHubProxy {
	client: TicketHubClient;
	server: TicketHubServer;
}

interface TicketHubServer {
	joinTicketGroup(ticketId: string): JQueryPromise<void>;
	updatingEditing(isEditing: boolean): JQueryPromise<void>;
}

interface TicketHubClient {
	Update: (connectionModels: TicketUserStateModel[]) => void;
}

interface TicketUserStateModel {
	UserName: string
	IsEditingTicket: string
}
