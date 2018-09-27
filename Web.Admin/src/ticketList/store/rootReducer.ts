import { combineReducers } from 'redux';
import { IAppState } from './models'
import { queueReducer } from './queue.ducks'
import { ticketTableReducer } from './ticketTable.ducks'

export default combineReducers<IAppState>({
	ticketDataTables: ticketTableReducer,
	queues: queueReducer
});
