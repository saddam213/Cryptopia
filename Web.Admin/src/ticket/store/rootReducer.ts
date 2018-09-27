import { combineReducers } from 'redux';
import { ticketReducer } from './ticket.ducks'
import { appStatusReducer } from './appStatus.ducks'
import { messagesReducer } from './messages.ducks'
import { connectedUsersReducer } from './connectedUsers.ducks'
import { IAppState } from './models'

export const rootReducer = combineReducers<IAppState>({
	ticket: ticketReducer,
	status: appStatusReducer,
	messages: messagesReducer,
	connectedUsers: connectedUsersReducer
});
