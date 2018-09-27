import { Dispatch, MiddlewareAPI, Action } from 'redux';
import { ticketHubFactory } from './signalrStoreSetup'
import * as appStatusActions from './appStatus.ducks'

export default function signalrMiddleware(store: MiddlewareAPI<any>) {

	let ticketHub = ticketHubFactory();

	return (next: any) => (anyAction: any) => {
		let returnValue = next(anyAction);
		let action: appStatusActions.AppStatusActionTypes = anyAction;

		switch (action.type) {
			case appStatusActions.AppStatusTypeKeys.CREATINGMESSAGE:
				let { isCreating } = action.payload;
				ticketHub.server.updatingEditing(isCreating).always(() => { });
				break;

			case appStatusActions.AppStatusTypeKeys.EDITING:
				let { isEditing } = action.payload;
				ticketHub.server.updatingEditing(isEditing).always(() => { });
				break;
		}
		return returnValue;
	}
}
