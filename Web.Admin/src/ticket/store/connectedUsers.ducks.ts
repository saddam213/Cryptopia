export enum ConnectedUsersTypeKeys {
	UPDATE = 'CONNECTEDUSERS_UPDATE',
}

export type ConnectedUsersActionTypes =
	| UpdateAction

export interface UpdateAction { type: ConnectedUsersTypeKeys.UPDATE; payload: TicketUserStateModel[] }

export function update(payload: TicketUserStateModel[]): ConnectedUsersActionTypes {
	return {
		type: ConnectedUsersTypeKeys.UPDATE,
		payload
	};
}

export function connectedUsersReducer(state: TicketUserStateModel[] = [], action: ConnectedUsersActionTypes): TicketUserStateModel[] {
	switch (action.type) {
		case ConnectedUsersTypeKeys.UPDATE:
			return action.payload;
		default:
			return state;
	}
}