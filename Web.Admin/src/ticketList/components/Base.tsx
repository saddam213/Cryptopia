import * as React from 'react';
import { connect } from 'react-redux';

import { IAppState, Queue } from '../store/models'
import { Tabs, TabModel } from '../../Reactopia/Tabs'
import TicketTable from './TicketTable';
import TicketTablePanel from './TicketTablePanel';

class Base extends React.Component<IProps, {}> {
	constructor(props, context) {
		super(props, context);
	}

	render() {

		const { queues } = this.props;

		const tabModels = queues.map((queue): TabModel => {
			return {
				name: queue.Name,
				content: (<TicketTablePanel queue={queue}/>)
			}
		});

		return (
			<div className="container-fluid">
				<Tabs tabs={tabModels}>
				</Tabs>
			</div>
		);

	}
}

interface IProps {
	queues: Queue[];
}

const mapStateToProps = (state: IAppState): IProps => ({
	queues: state.queues
});

export default connect(mapStateToProps)(Base);