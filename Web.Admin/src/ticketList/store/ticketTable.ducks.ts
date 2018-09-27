import { Dispatch } from 'redux';
import * as _ from 'lodash';
import * as models from '../store/models'
import { isLoading, hasErrored } from '../../Reactopia/DataTable'
import { getRazorModel } from '../common'
import { IDataTableHashMap, dataTableArrayReducer } from '../../Reactopia/DataTable'


export enum TicketTableTypeKeys {
	MOVE_TICKET = 'TICKETTABLE_MOVE_TICKET',
}

export type TicketTableActionTypes =
	| { type: TicketTableTypeKeys.MOVE_TICKET; payload: { tableId: string, ticketId: number, toQueueId: number } };

export const moveTicket = (tableId: string, ticketId: number, toQueueId: number): TicketTableActionTypes =>
	({ type: TicketTableTypeKeys.MOVE_TICKET, payload: { tableId, ticketId, toQueueId } });

export function ticketTableReducer(state: IDataTableHashMap = {}, action: TicketTableActionTypes): IDataTableHashMap {
	switch (action.type) {

		case TicketTableTypeKeys.MOVE_TICKET:
			const { tableId, ticketId, toQueueId } = action.payload;

			const table = state[tableId];
			const newItems = table.id == models.SepcialQueueType.ALL
				? table.items.map((ticket: models.Ticket) => {
					return ticket.Id != ticketId ? ticket : { ...ticket, QueueId: toQueueId }
				})
				: _.remove(table.items, (ticket: models.Ticket) => ticket.Id != ticketId);


			const newTable = {
				...table,
				items: newItems
			}

			return {
				...state,
				[tableId]: newTable
			}

		default:
			return dataTableArrayReducer(state, (action as any));
	}
}

export function moveTicketAndUpdate(ticket: models.Ticket, tableId: string, toQueueId: number) {
	const { moveTicketAction } = getRazorModel();
	return (dispatch: Dispatch<TicketTableActionTypes>, getState: () => models.IAppState) => {

		const { queues } = getState();

		if (ticket.QueueId == toQueueId) {
			return;
		}

		const data = {
			ticketId: ticket.Id,
			queueId: toQueueId
		};

		dispatch(isLoading(tableId, true));

		$.ajax({
			type: "POST",
			url: moveTicketAction,
			data: data,
		})
		.done(data => {
			dispatch(isLoading(tableId, false));
			dispatch(moveTicket(tableId, ticket.Id, toQueueId));
		})
		.fail(e => {
			dispatch(hasErrored(tableId, true))
		});

	}

}