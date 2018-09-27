import * as models from './models'

export enum MessagesTypeKeys {
	UPDATE = 'MESSAGES_UPDATE',
}

export type MessagesActionTypes =
	| UpdateAction

export interface UpdateAction { type: MessagesTypeKeys.UPDATE; payload: models.Message[] }

export function update(messages: models.Message[]): MessagesActionTypes {
	return {
		type: MessagesTypeKeys.UPDATE,
		payload: messages
	};
}

export function messagesReducer(state: models.Message[] = [], action: MessagesActionTypes): models.Message[] {
	switch (action.type) {

		case MessagesTypeKeys.UPDATE:
			return action.payload;

		default:
			return state;
	}
}

class CreateMessageModel {
	TicketId: number
	Message: string
}
