import * as React from 'react';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as moment from 'moment'
import { closeTicketAndUpdate, openTicketAndUpdate, modalEditTicket } from '../store/ticket.thunks'
import { getRazorModel } from '../common'

import { Ticket, IAppState, SupportTicketStatus } from '../store/models'

class TicketDetails extends React.Component<IProps & IDispatchProps, {}> {
	constructor(props, context) {
		super(props, context);
	}

	render() {

		const { ticket } = this.props;

		var tags = ticket.Tags.map((tag, i) => <span className="label label-info" key={i}>{tag}</span>);

		return (

			<div className="panel panel-default">
				<div className="panel-heading">
					<div className="row">
						<div className="col-lg-6" ><h4>Ticket Information</h4></div>
						<div className="col-lg-6" >
							<div className="btn-group pull-right">
								{this.editTicketButton}{this.closeTicketButton}
							</div>
						</div>
					</div>
				</div>
				<div className="panel-body">
					<ul  className="list-group">

						<li className="list-group-item">UserName:
							<span className="pull-right">
								<a href={`${getRazorModel().userAction}?userName=${ticket.UserName}`} target="_blank">{ticket.UserName}</a>
							</span>
						</li>
						<li className="list-group-item">Email:
							<span className="pull-right">{ticket.Email}</span>
						</li>
						<li className="list-group-item">Status:
							<span className="pull-right label label-primary">{this.displayStatus}</span>
						</li>

						<li className="list-group-item">Category:
							<span className="pull-right">{ticket.Category}</span>
						</li>

						<li className="list-group-item">Queue:
							<span className="pull-right">{ticket.Queue}</span>
						</li>

						<li className="list-group-item">Tags:
							<span className="pull-right">{tags}</span>
						</li>

						<li className="list-group-item">Opened:
							<span className="pull-right">{moment(ticket.Created).format('DD/MM/YYYY h:mm A')}</span>
						</li>

						<li className="list-group-item">Last Update:
							<span className="pull-right">{moment(ticket.LastUpdate).format('DD/MM/YYYY h:mm A')}</span>
						</li>

					</ul>

					<h4>Description:</h4>

					<pre>{ticket.Description}</pre>

				</div>
			</div>
		);

	}

	get editTicketButton(): JSX.Element {
		const { ticket, edit } = this.props;

		if (ticket.Status == SupportTicketStatus.Closed) {
			return ( <div></div> )
		} else {
			return (
				<button
					className="btn btn-info"
					onClick={edit}
				>Edit</button>
			)
		}

	}

	get closeTicketButton(): JSX.Element {
		const { ticket, close, reopen } = this.props;

		if (ticket.Status == SupportTicketStatus.Closed) {
			return (
				<button
					className="btn btn-info"
					onClick={reopen}
				>Reopen</button>
			)
		} else {
			return (
				<button
					className="btn btn-danger"
					onClick={close}
				>Close</button>
			)
		}

	}

	get displayStatus(): string {
		const { ticket } = this.props;

		let displayStatus: string = "";
		switch (ticket.Status) {
			case SupportTicketStatus.UserReply: {
				displayStatus = "Awaiting admin reply.";
				break;
			}
			case SupportTicketStatus.AdminReply: {
				displayStatus = "Admin replied.";
				break;
			}
			default: {
				displayStatus = ticket.Status;
				break;
			}
		}
		return displayStatus;
	}

}

interface IProps {
	ticket: Ticket;
}

interface IDispatchProps {
	close: () => void;
	reopen: () => void;
	edit: () => void;
}

const mapStateToProps = (state: IAppState): IProps => ({
	ticket: state.ticket
});

const mapDispatchToProps = (dispatch: Dispatch<any>): IDispatchProps => ({
	close: () => dispatch(closeTicketAndUpdate()),
	reopen: () => dispatch(openTicketAndUpdate()),
	edit: () => dispatch(modalEditTicket())
});

export default connect(mapStateToProps, mapDispatchToProps)(TicketDetails);
