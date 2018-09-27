import * as React from "react";
import { render } from "react-dom";
import { createStore, Store, applyMiddleware } from "redux";
import thunk from 'redux-thunk';
import { Provider } from "react-redux";
import { IAppState, Ticket } from './store/models'
import { rootReducer } from './store/rootReducer'
import { signalRStart } from './store/signalrStoreSetup';
import { getRazorModel } from './common';
import { appStatusInitialState } from './store/appStatus.ducks'
import Base from './components/Base';
import signalrMiddleware from './store/signalrMiddleware'

const intialState: IAppState = {
	ticket: getRazorModel().ticketViewModel.Ticket,
	messages: getRazorModel().ticketViewModel.Messages,
	status: appStatusInitialState,
	connectedUsers: []
};

let store: Store<IAppState> = createStore(rootReducer, intialState, applyMiddleware(thunk, signalrMiddleware));

signalRStart(store, () => {
	render(

		<Provider store={store}>
			<Base />
		</Provider>,

		document.getElementById("ticket-app")
	)
});
