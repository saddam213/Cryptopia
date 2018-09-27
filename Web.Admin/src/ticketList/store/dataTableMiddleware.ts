import { Dispatch, MiddlewareAPI, Action } from 'redux';
import * as actions from '../../Reactopia/DataTable/datatable.ducks'
import * as dataTableModels from '../../Reactopia/DataTable/models'
import * as models from './models'

export default function dataTableMiddleware(store: MiddlewareAPI<any>) {

	return (next: any) => (action: any) => {

		let returnValue = next(action);
		let { ticketDataTables } = store.getState() as models.IAppState;

		switch ((action as actions.ActionTypes).type) {
			case actions.TypeKeys.CREATE:
			case actions.TypeKeys.REFRESH:
			case actions.TypeKeys.SEARCH:
			case actions.TypeKeys.SORT:
			case actions.TypeKeys.PAGENEXT:
			case actions.TypeKeys.PAGEPREVIOUS:
			case actions.TypeKeys.GOTOPAGE:
			case actions.TypeKeys.UPDATE_REQUESTDATA:

				const model = ticketDataTables[(action as actions.ActionTypes).id];

				const apiModel = dataTableModels.DataTableApiModelFactory(model);
				var data = (model.requestMapper == null || model.requestMapper == undefined)
					? { model: apiModel }
					: model.requestMapper(apiModel, model.dynamicRequestData);

				$.ajax({
					type: "POST",
					url: model.url,
					data: data,
				})
				.done(data => {
					store.dispatch(actions.isLoading(model.id, false));
					var dataObject: dataTableModels.DataTableApiResponseModel = JSON.parse(data);
					store.dispatch(actions.fetchDataSuccess(model.id, dataObject))
				})
				.fail(e => {
					store.dispatch(actions.hasErrored(model.id, true))
				});
		}

		return returnValue;
	}
}
