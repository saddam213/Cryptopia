import * as React from 'react';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';

import * as moment from 'moment'

import { closeTicketAndUpdate, openTicketAndUpdate, modalEditTicket } from '../store/ticket.thunks'
import { getRazorModel } from '../common'

import { Ticket, IAppState, SupportTicketStatus } from '../store/models'

class OpenTickets extends React.Component<IProps, {}> {
	constructor(props, context) {
		super(props, context);
	}

	render() {
		return (
			<div className="panel panel-info">
				<div className="panel-heading">
					<div className="row">
						<div className="col-lg-12" ><h4>Open Tickets</h4></div>
					</div>
				</div>
				<div className="panel-body">
					{this.openTicketItems}
				</div>
			</div>
		);
	}

	get openTicketItems(): JSX.Element {
		const { ticket } = this.props;

		if (ticket.OpenTickets.length == 0) {
			return <h4>No other open tickets</h4>;
		} else {

			var openTicketItems = ticket.OpenTickets.map((t, i) => {
				return (
					<a href={`${getRazorModel().ticketDetailsAction}?id=${t.Id}`} target="_blank" className="list-group-item item-small" key={t.Id}>
						<div className="row">
							<div className="col-lg-6">
								<h6>{`#${t.Id} - ${t.Title}`}</h6>
							</div>
							<div className="col-lg-6">
								<h6 className="pull-right">{moment(t.Created).format('DD/MM/YYYY h:mm A')}</h6>
							</div>
						</div>
					</a>
				);
			});

			return (
				<div className="list-group">
					{openTicketItems}
				</div>
			);
		}
	}
}

interface IProps {
	ticket: Ticket;
}

const mapStateToProps = (state: IAppState): IProps => ({
	ticket: state.ticket
});

export default connect(mapStateToProps)(OpenTickets);