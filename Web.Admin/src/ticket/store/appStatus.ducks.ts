import { Dispatch } from 'redux';
import { AppStatus} from './models'
import { getRazorModel } from '../common'

export enum AppStatusTypeKeys {
	LOADING = 'APPSTATUS_LOADING',
	ERRORED = 'APPSTATUS_ERRORED',
	CREATINGMESSAGE = 'APPSTATUS_CREATINGMESSAGE',
	EDITING = 'APPSTATUS_EDITING',
}

export type AppStatusActionTypes = 
	| { type: AppStatusTypeKeys.LOADING; payload: boolean } //LoadingAction
	| { type: AppStatusTypeKeys.ERRORED; } //ErroredAction
	| { type: AppStatusTypeKeys.CREATINGMESSAGE; payload: { isCreating: boolean } } //CreatingMessageAction
	| {type: AppStatusTypeKeys.EDITING; payload: {isEditing: boolean, messageId: number}} //EditingAction

export function errored(): AppStatusActionTypes {
	return {
		type: AppStatusTypeKeys.ERRORED
	};
}

export function loading(isLoading: boolean): AppStatusActionTypes {
	return {
		type: AppStatusTypeKeys.LOADING,
		payload: isLoading
	};
}

export function creatingMessage(isCreating: boolean): AppStatusActionTypes {
	return {
		type: AppStatusTypeKeys.CREATINGMESSAGE,
		payload: {
			isCreating
		}
	};
}

export function editing(isEditing: boolean, messageId: number = undefined): AppStatusActionTypes {
	return {
		type: AppStatusTypeKeys.EDITING,
		payload: {
			isEditing,
			messageId
		}
	};
}

export const appStatusInitialState: AppStatus = {
	isLoading: false,
	hasErrored: false,
	isCreatingMessage: false,
	isEditing: false,
}

export function appStatusReducer(state: AppStatus = appStatusInitialState, action: AppStatusActionTypes): AppStatus {
	switch (action.type) {

		case AppStatusTypeKeys.ERRORED:
			return {
				...appStatusInitialState,
				hasErrored: true,
			}

		case AppStatusTypeKeys.LOADING:
			return {
				...state,
				isLoading: action.payload,
			}

		case AppStatusTypeKeys.CREATINGMESSAGE:
			let { isCreating } = action.payload;
			return {
				...appStatusInitialState,
				isCreatingMessage: isCreating
			}

		case AppStatusTypeKeys.EDITING:
			let { isEditing, messageId } = action.payload;
			return {
				...appStatusInitialState,
				isEditing: isEditing,
				messageId: messageId
			}

		default:
			return state;
	}
}
