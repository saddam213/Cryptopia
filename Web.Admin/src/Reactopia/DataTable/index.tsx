import * as _ from 'lodash';
import * as React from 'react';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import * as models from './models';
import * as actions from './datatable.ducks';

export * from './models';
export * from './datatable.ducks';

export interface IComponentProps {
	headerDefinitions: models.HeaderDefinition[];
	rowMapper: (item: any) => JSX.Element;

	dataMapper?: (value: any, index: number, array: any[]) => any;
	requestMapper?: (apiModel: models.DataTableApiModel, dynamicRequestData: any) => any;

	displayLength: number;
	url: string;
	id: string;
	dataTable: models.DataTableModel;
}

export interface IDispatchProps {
	create: (model: models.CreateModel) => void
	search: (id: string, searchString: string) => void
	sort: (id: string, index: number) => void
	pagePrevious: (id: string) => void
	pageNext: (id: string) => void
	gotoPage: (id: string, page: number) => void
}

export type DataTableProps = IComponentProps & IDispatchProps

class DataTable<T> extends React.Component<DataTableProps, {}> {

	constructor(props: DataTableProps, context: any) {
		super(props, context);

		const { create} = this.props;
		if (!this.isCreated) {
			create(this.createModel)
		}
	}

	render() {
		const { dataTable } = this.props;

		if (!this.isCreated) {
			return <div />
		}
		else if (dataTable.hasErrored) {
			return <p>Sorry! There was an error loading the items</p>;
		}

		return (
			<div>

				<div className="row">
					<div className="col-lg-2 pull-right">
						{this.searchBox}
					</div>
				</div>

				<table className="table table-condensed table-striped table-hover dataTable no-footer">
					<thead className="thead-default">
						<tr>
							{ this.Headers }
						</tr>
					</thead>
					<tbody>
						{ this.items }
					</tbody>
				</table>
				{ this.pager }

			</div>
		);

	}

	get items(): JSX.Element[] {
		const { dataTable, rowMapper } = this.props;
		return dataTable.items.map(i => rowMapper(i));
	}

	get Headers(): JSX.Element[] {

		const { dataTable, sort} = this.props;

		var elements: JSX.Element[] = [];

		var sortingFuncGenerator = function (sortIndex): () => void {
			return () => sort(dataTable.id, sortIndex)
		}

		for (var headerIndex = 0; headerIndex < dataTable.headers.length; headerIndex++) {
			let header = dataTable.headers[headerIndex];

			let sortingClass = "";
			let sortingFunc = () => { };

			if (header.canSort) {
				sortingClass = "sorting"
				sortingFunc = sortingFuncGenerator(header.index);

				if (dataTable.sortOrder[0] == header.index)
					sortingClass += "_" + dataTable.sortState[header.index];
			}

			elements[headerIndex] = <th className={sortingClass} onClick={sortingFunc} key={header.index}>{header.name}</th>;
		}

		return elements;
	}

	get searchBox(): JSX.Element {
		const { dataTable, search } = this.props;

		var delayTimer;
		function doSearch(event: React.ChangeEvent<HTMLInputElement>) {
			clearTimeout(delayTimer);
			var text = event.target.value;
			delayTimer = setTimeout(() => search(dataTable.id, text), 1000);
		}

		return <input type="text" className="search form-control" placeholder="Search ..."
			defaultValue={dataTable.searchString}
			onChange={(e) => { doSearch(e) }} />
	}

	get pager(): JSX.Element {

		const { dataTable, pageNext, pagePrevious, gotoPage } = this.props;

		var previousIsDisabled = dataTable.itemPagerStart == 0;
		var nextIsDisabled = dataTable.itemPagerStart + dataTable.displayLength >= dataTable.totalItems;

		var createPageItemFn = function (page) {
			var pageLink = page == currentPage
				? <a>{page + 1}</a>
				: <a className="page-link" href="#" onClick={() => gotoPage(dataTable.id, page)}>{page + 1}</a>;

			return <li className={`page-item ${page == currentPage ? ' active' : ''}`} key={page + 1}>{pageLink}</li>;
		}

		//Pagination Logic
		let totalPages = Math.ceil(dataTable.totalItems / dataTable.displayLength);
		let currentPage = (dataTable.itemPagerStart / dataTable.displayLength);

		const halfPageSize = 5;
		const pageSize = halfPageSize * 2;

		let pages: JSX.Element[] = [];

		if (totalPages <= pageSize) {

			for (var i = 0; i < totalPages; i++) {
				pages[i] = createPageItemFn(i)
			}

		} else {
			let lhsDistance = currentPage;
			let rhsDistance = totalPages - (currentPage + 1);

			let isLhsLimited = lhsDistance < halfPageSize;
			let isRhsLimited = rhsDistance < halfPageSize;

			let lhsSize = isLhsLimited ? lhsDistance : halfPageSize + (isRhsLimited ? halfPageSize - rhsDistance : 0);
			let rhsSize = isRhsLimited ? rhsDistance : halfPageSize + (isLhsLimited ? halfPageSize - lhsDistance : 0);

			let startPage = currentPage - lhsSize;
			let endPage = currentPage + rhsSize;

			let lhsPages: JSX.Element[] = currentPage == 0 ? [] : Array.apply(null, Array(lhsSize)).map((x, i) => {
				let page = startPage + i;
				return createPageItemFn(page);
			});

			let rhsPages: JSX.Element[] = currentPage == totalPages ? [] : Array.apply(null, Array(rhsSize)).map((x, i) => {
				let page = currentPage + (i + 1);
				return createPageItemFn(page);
			});

			let startPageItems: JSX.Element[] = [];
			let endPageItems: JSX.Element[] = [];

			if (startPage > 0)
				startPageItems.push(createPageItemFn(0))
			if (startPage > 1)
				startPageItems.push(<li className={`page-item disabled`} key={'start seperator'}><a>...</a></li>)

			if (endPage < (totalPages - 2))
				endPageItems.push(<li className={`page-item disabled`} key={'end seperator'}><a>...</a></li>)
			if (endPage < (totalPages - 1))
				endPageItems.push(createPageItemFn(totalPages - 1))

			pages = [
				...startPageItems,
				...lhsPages,
				createPageItemFn(currentPage),
				...rhsPages,
				...endPageItems
			];

		}

		return (
			<div className="row">
				<div className="col-sm-5">
						{dataTable.totalItems} items
				</div>
				<div className="col-sm-7">
					<nav className="form-group pull-right">
						<ul className="pagination">

							<li className={`page-item ${previousIsDisabled ? ' disabled' : ''}`} key={'previous'}>
								<a className="page-link" href="#" onClick={() => previousIsDisabled ? null : pagePrevious(dataTable.id) }>Previous</a>
							</li>

							{pages}

							<li className={`page-item ${nextIsDisabled ? ' disabled' : ''}`} key={'next'}>
								<a className="page-link" href="#" onClick={() => nextIsDisabled ? null : pageNext(dataTable.id)}>Next</a>
							</li>

						</ul>
					</nav>
				</div>
			</div>
		)
	}

	get createModel(): models.CreateModel {
		const { id, url, headerDefinitions, displayLength, dataMapper, requestMapper } = this.props;
		return {
			id,
			url,
			headerDefinitions,
			displayLength,
			dataMapper,
			requestMapper
		}
	}

	get isCreated() {
		return this.props.dataTable != null && this.props.dataTable != undefined;
	}

}

const mapStateToProps = (state: any, props: IComponentProps): IComponentProps => {
	return props;
};

const mapDispatchToProps = (dispatch: Dispatch<any>): IDispatchProps => ({
	create: (model) => dispatch(actions.create(model)),
	search: (id, searchString) => dispatch(actions.search(id, searchString)),
	sort: (id, index) => dispatch(actions.sort(id, index)),
	pageNext: (id) => dispatch(actions.pageNext(id)),
	pagePrevious: (id) => dispatch(actions.pagePrevious(id)),
	gotoPage: (id, page) => dispatch(actions.gotoPage(id, page))
});

export default connect(mapStateToProps, mapDispatchToProps)(DataTable);
