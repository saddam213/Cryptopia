import * as React from 'react';
import * as _ from 'lodash'
import TicketTable from './TicketTable';
import { Queue, IAppState } from '../store/models'
import { connect } from 'react-redux';

interface IProps {
	queue: Queue;
}

export default class TicketTablePanel extends React.Component<IProps, {}> {

	render() {

		const { queue } = this.props;

		return (
			<div className="panel panel-default">
				<div className="panel-heading">Tickets</div>
				<div className="panel-body">
					<TicketTable
						queue={queue}
					/>
				</div>
			</div>)

	}
}
