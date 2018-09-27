import { Dispatch } from 'redux';
import * as models from './models'
import * as statusActions from './appStatus.ducks'
import { getRazorModel } from '../common'

export function createMessage(message: string, isAdminMessage: boolean = false) {
	const { createMessageAction } = getRazorModel();
	return (dispatch: Dispatch<{}>, getState: () => models.IAppState) => {
		const { ticket } = getState();
		const data = {
			ticketId: ticket.Id,
			message,
			isAdminMessage
		};

		ajaxPostMethod(dispatch, createMessageAction, data, (data) => {
			dispatch(statusActions.editing(false));
		});
	}
}

export function deleteMessage(messageId: number) {
	const { deleteMessageAction } = getRazorModel();
	return (dispatch: Dispatch<{}>, getState: () => models.IAppState) => {
		const { ticket } = getState();

		const data = {
			ticketId: ticket.Id,
			messageId
		};

		ajaxPostMethod(dispatch, deleteMessageAction, data, (data) => {
			//TODO handle message delete 
		});
	}
}

export function publishMessage(messageId: number) {
	const { publishMessageAction } = getRazorModel();
	return (dispatch: Dispatch<{}>, getState: () => models.IAppState) => {
		const { ticket } = getState();

		const data = {
			ticketId: ticket.Id,
			messageId
		};

		ajaxPostMethod(dispatch, publishMessageAction, data, (data) => {
			//TODO handle message publish
		});
	}
}

export function editMessage(messageId: number, message: string) {
	const { editMessageAction } = getRazorModel();
	return (dispatch: Dispatch<{}>, getState: () => models.IAppState) => {
		const { ticket } = getState();

		const data = {
			ticketId: ticket.Id,
			messageId,
			message
		};

		ajaxPostMethod(dispatch, editMessageAction, data, (data) => {
			dispatch(statusActions.editing(false));
			//TODO handle message edit
		});
	}
}

function ajaxPostMethod(dispatch: Dispatch<{}>, url: string, data: {}, onDone: (data: any) => void) {
	dispatch(statusActions.loading(true));
	$.ajax({
		type: "POST",
		url: url,
		data: data,
	})
		.done((data) => {
			dispatch(statusActions.loading(false));
			onDone(data);
		})
		.fail(e => {
			dispatch(statusActions.errored());
		});
}
