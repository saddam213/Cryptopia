import { Store } from "redux";
import { IAppState, Ticket } from './models';
import { updateTicket } from './ticket.thunks'
import { update } from './connectedUsers.ducks'

export function signalRStart(store: Store<IAppState>, callback: Function) {
	const { ticket } = store.getState();

	let supportHub = supportHubFactory();
	let ticketHub = ticketHubFactory();

	supportHub.client.TicketUpdated = (ticketId: number) => {
		if (ticketId == ticket.Id) {
			store.dispatch(updateTicket());
		}
	}

	ticketHub.client.Update = (connectionModels) => {
		store.dispatch(update(connectionModels));
	}

	$.connection.hub.start(() => {
		ticketHub.server.joinTicketGroup(ticket.Id.toString());
		callback();
	});
}

let _supportHub: SupportHubProxy = null;
let _ticketHub: TicketHubProxy = null;

export function supportHubFactory() {
	if (!_supportHub) {
		_supportHub = $.connection.supportHub;
	}
	return _supportHub;
}

export function ticketHubFactory() {
	if (!_ticketHub) {
		_ticketHub = $.connection.ticketHub;
	}
	return _ticketHub;
}
