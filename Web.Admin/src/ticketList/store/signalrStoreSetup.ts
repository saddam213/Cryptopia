import { Store } from "redux";
import { IAppState, Ticket } from './models';
import { refresh } from '../../Reactopia/DataTable'

export function signalRStart(store: Store<IAppState>, callback: Function) {
	let supportHub = supportHubFactory();

	supportHub.client.TicketUpdated = (ticketId: number) => {

		const { ticketDataTables } = store.getState();
		//TODO: Update logic as this is a bit of a waste. Not very efficient

		for (let key in ticketDataTables) {
			if (ticketDataTables.hasOwnProperty(key)) {
				let table = ticketDataTables[key];
				if (table != null || table != undefined) {
					store.dispatch(refresh(ticketDataTables[key].id));
				}
			}
		}

	}

	$.connection.hub.start(() => callback());
}

let _supportHub: SupportHubProxy = null;

export function supportHubFactory() {
	if (!_supportHub) {
		_supportHub = $.connection.supportHub;
	}
	return _supportHub;
}
