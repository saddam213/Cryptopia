export enum SortingType {
	desc = 'desc',
	asc = 'asc'
}

export interface IDataTableHashMap {
	[tableId: string]: DataTableModel
}

export class DataTableModel {
	id: string;
	url: string;
	headers: HeaderDefinition[];
	dataMapper?: (value: any, index: number, array: any[]) => any;
	requestMapper?: (apiModel: DataTableApiModel, dynamicRequestData: any) => any;
	searchString: string;
	sortState: SortingType[];
	sortOrder: number[];
	isLoading: boolean;
	hasErrored: boolean;
	items: any[];
	displayLength: number;
	totalItems: number;
	itemPagerStart: number;
	dynamicRequestData: any; 
}

export class HeaderDefinition {

	constructor(name, index = null, canSort = true, canSearch = true, sortByDefault = false) {
		this.name = name;
		this.index = index;
		this.canSort = canSort;
		this.canSearch = canSearch;
		this.sortByDefault = sortByDefault;
	}

	name: string;
	canSort: boolean;
	canSearch: boolean;
	sortByDefault: boolean; 
	index?: number;
}

export class DataTableApiResponseModel {
	aaData: any[]
	iTotalDisplayRecords: number
	iTotalRecords: number
	sEcho: number
}

export class DataTableApiModel {
	bEscapeRegex: false
	bEscapeRegexColumns: boolean[]
	bSearchable: boolean[]
	bSortable: boolean[]
	iColumns: number
	iDisplayLength: number
	iDisplayStart: number
	iSortCol: number[]
	iSortingCols: number
	sColumnNames: string[]
	sEcho: number
	sSearch: string
	sSearchValues: string[]
	sSortDir: string[]
}

export function DataTableApiModelFactory(tableModel: DataTableModel): DataTableApiModel {
	const iColumns = tableModel.headers.length;

	let sSortDir = tableModel.sortOrder.map((v, i) => tableModel.sortState[v]);

	let bSearchable: boolean[] = [];
	let bSortable: boolean[] = [];
	for (var i = 0; i < tableModel.headers.length; i++) {
		var header = tableModel.headers[i];
		if (header.index != null) {
			bSearchable[header.index] = header.canSearch;
			bSortable[header.index] = header.canSort;
		}
	}

	let model: DataTableApiModel = {
		bEscapeRegex: false,
		bEscapeRegexColumns: tableModel.headers.map(x => false),
		bSearchable: bSearchable,
		bSortable: bSortable,
		iColumns,
		iDisplayLength: tableModel.displayLength,
		iDisplayStart: tableModel.itemPagerStart,
		iSortCol: tableModel.sortOrder,
		iSortingCols: 1,
		sColumnNames: [],
		sEcho: 1,
		sSearch: tableModel.searchString,
		sSearchValues: tableModel.headers.map(x => ""),
		sSortDir,
	}
	return model;
}

export class CreateModel {
	id: string;
	url: string;
	headerDefinitions: HeaderDefinition[];
	displayLength: number;
	dataMapper?: (value: any, index: number, array: any[]) => any;
	requestMapper?: (apiModel: DataTableApiModel, dynamicRequestData: any) => any;
}