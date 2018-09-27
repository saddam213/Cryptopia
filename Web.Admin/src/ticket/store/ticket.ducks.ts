import * as models from './models'

export enum TicketActionTypeKeys {
	UPDATE = 'TICKET_UPDATE',
	SETSTATUS = 'TICKET_SET_STATUS'
}

export type TicketActionTypes =
	| { type: TicketActionTypeKeys.UPDATE; payload: models.Ticket } //UpdateAction
	| { type: TicketActionTypeKeys.SETSTATUS; payload: models.SupportTicketStatus } //SetStatusAction

export const update = (ticket: models.Ticket): TicketActionTypes =>
	({ type: TicketActionTypeKeys.UPDATE, payload: ticket });

export const setStatus = (status: models.SupportTicketStatus): TicketActionTypes =>
	({ type: TicketActionTypeKeys.SETSTATUS, payload: status });

const initialState = null;

export function ticketReducer(state: models.Ticket = initialState, action: TicketActionTypes): models.Ticket {
	switch (action.type) {

		case TicketActionTypeKeys.UPDATE:
			return action.payload;

		case TicketActionTypeKeys.SETSTATUS:
			return {
				...state,
				Status: action.payload
			};

		default:
			return state;
	}
}
