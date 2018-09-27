import * as models from './models'

export enum TypeKeys {
	CREATE = 'DATATABLE_CREATE',
	UPDATE = 'DATATABLE_UPDATE',
	REFRESH = 'DATATABLE_REFRESH',
	SEARCH = 'DATATABLE_SEARCH',
	SORT = 'DATATABLE_SORT',
	PAGENEXT = 'DATATABLE_PAGENEXT',
	PAGEPREVIOUS = 'DATATABLE_PAGEPREVIOUS',
	GOTOPAGE = 'DATATABLE_GOTOPAGE',
	UPDATE_REQUESTDATA = 'DATATABLE_UPDATE_REQUESTDATA',
	HAS_ERRORED = 'DATATABLE_HAS_ERRORED',
	IS_LOADING = 'DATATABLE_IS_LOADING',
	FETCH_DATA_SUCCESS = 'DATATABLE_FETCH_DATA_SUCCESS'
}

export type ActionTypes =
	| { id: string; type: TypeKeys.CREATE; payload: models.DataTableModel } //CreateAction
	| { id: string; type: TypeKeys.UPDATE; payload: models.DataTableModel } //UpdateAction
	| { id: string; type: TypeKeys.REFRESH; } //RefreshAction

	| { id: string; type: TypeKeys.SEARCH; payload: string } //SearchAction
	| { id: string; type: TypeKeys.SORT; payload: number } //SortAction
	| { id: string; type: TypeKeys.PAGENEXT; } //PageNextAction
	| { id: string; type: TypeKeys.PAGEPREVIOUS; } //PagePreviousAction
	| { id: string; type: TypeKeys.GOTOPAGE; payload: number; } //GotoPageAction
	| { id: string, type: TypeKeys.UPDATE_REQUESTDATA; payload: any; } //UpdateRequestDataAction

	| { id: string, type: TypeKeys.HAS_ERRORED; payload: boolean; } //HasErroredAction
	| { id: string, type: TypeKeys.IS_LOADING; payload: boolean; } //IsLoadedAction
	| { id: string, type: TypeKeys.FETCH_DATA_SUCCESS; payload: models.DataTableApiResponseModel; } //FetchDataAction

export const create = (createModel: models.CreateModel): ActionTypes => {
	const { id, url, displayLength, headerDefinitions, dataMapper, requestMapper } = createModel;

	var sortIndex = headerDefinitions.filter(x => x.sortByDefault)[0].index;

	var sortOrder = [sortIndex, ... new Array(headerDefinitions.length -1)];

	var sortState = new Array(headerDefinitions.length);
	sortState[sortIndex] = models.SortingType.desc;

	var newState: models.DataTableModel = {
		id,
		url,
		displayLength,
		headers: headerDefinitions,
		searchString: "",
		sortState,
		sortOrder,
		isLoading: false,
		hasErrored: false,
		items: [],
		totalItems: 0,
		itemPagerStart: 0,
		dataMapper,
		requestMapper,
		dynamicRequestData: null
	}
	return { type: TypeKeys.CREATE, id: id, payload: newState }
}

export const update = (dataTable: models.DataTableModel): ActionTypes =>
	({ type: TypeKeys.UPDATE, id: dataTable.id, payload: dataTable });

export const refresh = (id: string): ActionTypes =>
	({ type: TypeKeys.REFRESH, id });

export const search = (id: string, searchString: string): ActionTypes =>
	({ type: TypeKeys.SEARCH, id, payload: searchString });

export const sort = (id: string, index: number): ActionTypes =>
	({ type: TypeKeys.SORT, id, payload: index });

export const pageNext = (id: string ): ActionTypes =>
	({ type: TypeKeys.PAGENEXT, id });

export const pagePrevious = (id: string ): ActionTypes =>
	({ type: TypeKeys.PAGEPREVIOUS, id });

export const gotoPage = (id: string, payload: number): ActionTypes =>
	({ type: TypeKeys.GOTOPAGE, id, payload });

export const updateRequestData = (id: string, payload: any): ActionTypes =>
	({ type: TypeKeys.UPDATE_REQUESTDATA, id, payload });

export const hasErrored = (id: string, payload: boolean): ActionTypes =>
	({ type: TypeKeys.HAS_ERRORED, id, payload });

export const isLoading = (id: string, payload: boolean): ActionTypes =>
	({ type: TypeKeys.IS_LOADING, id, payload });

export const fetchDataSuccess = (id: string, payload: models.DataTableApiResponseModel): ActionTypes =>
	({ type: TypeKeys.FETCH_DATA_SUCCESS, id, payload });

const initialState = {};

export function dataTableArrayReducer(state: models.IDataTableHashMap = initialState, action: ActionTypes): models.IDataTableHashMap {
	switch (action.type) {

		case TypeKeys.CREATE:
			const newState: models.DataTableModel = action.payload;
			return {
				...state,
				[action.id]: newState
			}

		default:
			return {
				...state,
				[action.id]: dataTableReducer(state[action.id], action)
			}
	}
}

function dataTableReducer(state: models.DataTableModel, action: ActionTypes): models.DataTableModel {
	switch (action.type) {

		case TypeKeys.UPDATE:
			return action.payload;

		case TypeKeys.SEARCH:
			return {
				...state,
				itemPagerStart: 0, //Sets pager to 0
				searchString: action.payload
			}


		case TypeKeys.SORT:
			let index = action.payload;
			let newSortType = state.sortState[index] == models.SortingType.desc ? models.SortingType.asc : models.SortingType.desc;

			let newSortState = new Array(state.sortState.length);
			newSortState[index] = newSortType;
			const newSortOrder = [index, ... new Array(state.sortState.length - 1)];

			return {
				...state,
				itemPagerStart: 0, //Sets pager to 0
				sortState: newSortState,
				sortOrder: newSortOrder
			}

		case TypeKeys.PAGENEXT:
			let newItemPagerStart = state.itemPagerStart + state.displayLength;
			let canPage = (newItemPagerStart) <= state.totalItems;

			if (canPage) {
				return {
					...state,
					itemPagerStart: newItemPagerStart
				}
			} else {
				return state;
			}

		case TypeKeys.PAGEPREVIOUS:
			if (state.itemPagerStart > 0) {
				let newItemPagerStart = state.itemPagerStart - state.displayLength;

				return {
					...state,
					itemPagerStart: newItemPagerStart >= 0 ? newItemPagerStart : 0
				}
			} else {
				return state;
			}

		case TypeKeys.GOTOPAGE:
			let page = action.payload;
			return {
				...state,
				itemPagerStart: page * state.displayLength
			}

		case TypeKeys.UPDATE_REQUESTDATA:
			return {
				...state,
				dynamicRequestData: action.payload
			}

		case TypeKeys.IS_LOADING:
			return {
				...state,
				isLoading: action.payload
			}

		case TypeKeys.HAS_ERRORED:
			return {
				...state,
				hasErrored: action.payload,
				isLoading: false
			}

		case TypeKeys.FETCH_DATA_SUCCESS:

			const { aaData, iTotalDisplayRecords } = action.payload;

			let items = (state.dataMapper == null || state.dataMapper == undefined)
				? aaData
				: aaData.map((value, index, array) => state.dataMapper(value, index, array));

			return {
				...state,
				items,
				totalItems: iTotalDisplayRecords,
				hasErrored: false,
				isLoading: false
			}

		default:
			return state;
	}
}
