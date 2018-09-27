import { Dispatch } from 'redux';
import * as models from './models'
import * as actions from './ticket.ducks'
import { getRazorModel } from '../common'
import * as statusActions from './appStatus.ducks'
import * as messageActions from './messages.ducks'
import * as ticketActions from './ticket.ducks'

export function updateTicket() {
	const { getTicketDetailsAction } = getRazorModel();
	return (dispatch: Dispatch<{}>, getState: () => models.IAppState) => {
		const { ticket } = getState();
		dispatch(statusActions.loading(true));
		$.ajax({
			type: "POST",
			url: getTicketDetailsAction,
			data: {
				ticketId: ticket.Id
			},
		})
			.done((model: models.TicketViewModel) => {
				dispatch(messageActions.update(model.Messages));
				dispatch(actions.update(model.Ticket));
				dispatch(statusActions.loading(false));
			})
			.fail(e => {
				dispatch(statusActions.errored());
			});
	}
}

export function closeTicketAndUpdate() {
	const { closeTicketAction } = getRazorModel();
	return (dispatch: Dispatch<{}>, getState: () => models.IAppState) => {
		dispatch(ticketActions.setStatus(models.SupportTicketStatus.Closed));

		const { ticket } = getState();
		dispatch(statusActions.loading(true));
		$.ajax({
			type: "POST",
			url: closeTicketAction,
			data: {
				ticketId: ticket.Id
			},
		})
			.done(data => {
				dispatch(statusActions.loading(false));
			})
			.fail(e => {
				dispatch(statusActions.errored());
			});
	}
}

export function openTicketAndUpdate() {
	const { reopenTicketAction } = getRazorModel();
	return (dispatch: Dispatch<{}>, getState: () => models.IAppState) => {
		const { ticket } = getState();
		dispatch(statusActions.loading(true));
		$.ajax({
			type: "POST",
			url: reopenTicketAction,
			data: {
				ticketId: ticket.Id
			},
		})
			.done(data => {
				dispatch(statusActions.loading(false));
			})
			.fail(e => {
				dispatch(statusActions.errored());
			});
	}
}

export function modalEditTicket() {
	const { updateTicketAction } = getRazorModel();
	return (dispatch: Dispatch<{}>, getState: () => models.IAppState) => {
		const { ticket } = getState();
		dispatch(statusActions.editing(true));
		openModalGet(updateTicketAction, { id: ticket.Id }, (modalData) => {
			dispatch(statusActions.editing(false));
			showMessage(modalData);
		});
	}
}
