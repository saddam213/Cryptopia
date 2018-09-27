import * as React from "react";
import { render } from "react-dom";
import { createStore, Store, applyMiddleware } from "redux";
import { Provider } from "react-redux";
import dataTableMiddleware from './store/dataTableMiddleware';
import thunk from 'redux-thunk';

import { signalRStart } from './store/signalrStoreSetup';
import { IAppState } from './store/models';
import rootReducer from './store/rootReducer';
import { getQueues } from './common';
import Base from "./components/Base";


const intialState: IAppState = {
	ticketDataTables: {},
	queues: getQueues()
};

let store: Store<IAppState> = createStore(rootReducer, intialState, applyMiddleware(thunk, dataTableMiddleware));

signalRStart(store, () => {
	render(

		<Provider store={store}>
			<Base />
		</Provider>,

		document.getElementById("ticket-list-app")
	)
});

