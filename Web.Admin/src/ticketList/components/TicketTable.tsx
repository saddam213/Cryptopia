import * as React from 'react';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import * as moment from 'moment'

import { Ticket, Queue, SepcialQueueType, IAppState } from '../store/models'
import DataTable, { HeaderDefinition, DataTableModel, updateRequestData } from '../../Reactopia/DataTable';
import QueueSelector from './QueueSelector';
import { getRazorModel } from '../common'

interface IComponentProps {
	queue: Queue
}

export interface IDispatchProps {
	updateRequestData: (id: string, data: any) => void
}

interface IStoreProps {
	dataTable: DataTableModel
}

class TicketTable extends React.Component<IComponentProps & IStoreProps & IDispatchProps, {}> {

	render() {
		const { queue, dataTable } = this.props;
		const tableId = queue.Id;

		//Header index is the index of the field in the dynamic projection
		const headerDefinitions = [
			new HeaderDefinition("Open", null, false, false),
			new HeaderDefinition("Id", 0),
			new HeaderDefinition("User Name", 1),
			new HeaderDefinition("Category", 2),
			new HeaderDefinition("Title", 3),
			new HeaderDefinition("Status", 4),
			new HeaderDefinition("Last Update", 5),
			new HeaderDefinition("Created", 6, true, true, true),
			new HeaderDefinition("Tags", 7, false, false)
		];

		headerDefinitions.push(new HeaderDefinition("Queue", headerDefinitions.length, false, false));

		const queueColumn = (t: Ticket): JSX.Element => {
			if (queue.Id == SepcialQueueType.CLOSED) {
				return <td>{t.QueueName}</td>;
			}
			return <td><QueueSelector ticket={t} tableId={tableId} /></td>;
		}

		var tagColumn = (t: Ticket): JSX.Element => {
			return (
				<td>
					{t.Tags.map((tag, i) => <span className="label label-info" key={i}>{tag}</span>)}
				</td> );
		}

		const rowMapper = (t: Ticket): JSX.Element => {
			return (
				<tr key={t.Id} className={"clickable-row"}>
					<td><a href={`${getRazorModel().ticketDetailsAction}?id=${t.Id}`} target="_blank"><i className="btn btn-sm btn-info fa fa-eye"></i></a></td>
					<td>{t.Id}</td>
					<td><a href={`${getRazorModel().userAction}?userName=${t.UserName}`} target="_blank">{t.UserName}</a></td>
					<td>{t.Category}</td>
					<td>{t.Title}</td>
					<td>{t.Status}</td>
					<td>{t.LastUpdate}</td>
					<td>{t.Opened}</td>
					{tagColumn(t)}
					{queueColumn(t)}
				</tr> )
		};

		const dataMapper = (t: Ticket): Ticket => {
			return {
				...t,
				Opened: moment(t.Opened).format('DD/MM/YYYY h:mm A'),
				LastUpdate: moment(t.LastUpdate).format('DD/MM/YYYY h:mm A')
			}
		}

		const requestMapper = (apiModel, data) => {
			return {
				model: {
					QueueId: queue.EntityId,
					IsClosed: queue.Id == SepcialQueueType.CLOSED,
					DataTablesModel: apiModel,
					TabSearch: data
				}
			}
		}

		return (

			<div>

				<div className="row">
					<div className="col-lg-2 pull-right">
						{this.searchTagBox}
					</div>
				</div>

				<DataTable
					headerDefinitions={headerDefinitions}
					url={getRazorModel().getOpenTicketsAction}
					rowMapper={rowMapper}
					id={tableId}
					displayLength={20}
					dataMapper={dataMapper}
					requestMapper={requestMapper}
					dataTable={dataTable}
				/>
			</div>
		);
	}

	get searchTagBox(): JSX.Element {
		const { dataTable, updateRequestData } = this.props;

		if (dataTable == null || dataTable == undefined) {
			return <div></div>;
		}

		var delayTimer;
		function doSearch(event: React.ChangeEvent<HTMLInputElement>) {
			clearTimeout(delayTimer);
			var text = event.target.value;
			delayTimer = setTimeout(() => updateRequestData(dataTable.id, text), 1000);
		}

		return (
			<div className="form-group">
				<input type="text" className="search form-control" placeholder="Search for Tag"
					defaultValue={dataTable.dynamicRequestData}
					onChange={(e) => { doSearch(e) }} />
			</div>
			)
	}

}

const mapStateToProps = (state: IAppState, props: IComponentProps): IComponentProps & IStoreProps => {
	return {
		...props,
		dataTable: state.ticketDataTables[props.queue.Id]
	}
};

const mapDispatchToProps = (dispatch: Dispatch<any>): IDispatchProps => ({
	updateRequestData: (id, data) => dispatch(updateRequestData(id, data))
});

export default connect(mapStateToProps, mapDispatchToProps)(TicketTable);
