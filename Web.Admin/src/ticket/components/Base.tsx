import * as React from 'react'
import { connect } from 'react-redux'
import { getRazorModel } from '../common'
import { Ticket, IAppState } from '../store/models'
import TicketDetails from './TicketDetails'
import OpenTickets from './OpenTickets'
import Messages from './Messages'

class Base extends React.Component<IProps, {}> {
	constructor(props, context) {
		super(props, context);
	}

	render() {
		let { userName } = getRazorModel();
		const { ticket, isLoading, connectedUsers, hasErrored } = this.props;

		if (hasErrored){
			return <p>Sorry! An Error has occured</p>;
		}

		const spinner = isLoading ? <i className="fa fa-spinner fa-spin fa-4x fa-fw pull-right"></i> : null;
		let titleText = `#${ticket.Id} - ${ticket.Title}`;

		let anyOtherUsersEditing = connectedUsers.some(x => x.IsEditingTicket && x.UserName != userName);
		let alertType = anyOtherUsersEditing ? "danger" : "info";

		let connectedUserLabels = connectedUsers.map(u => {
			var labelType = u.IsEditingTicket ? "warning" : "info";
			return <span style={{ marginRight: 10 }} className={`label label-${labelType}`} key={u.UserName}>{`${u.UserName} : ${u.IsEditingTicket ? "Editing" : "Viewing"}`}</span>
		})

		return (
			<div className="panel">

				<div className="panel-heading">
					<div className="row">
						<div className="col-lg-5">
							<h2 style={{ margin: 0 }}>{titleText}</h2>
						</div>
						<div className="col-lg-2">
							{spinner}
						</div>
						<div className={`alert alert-${alertType} col-lg-5`} role="alert">
							{connectedUserLabels}
						</div>
					</div>
				</div>

				<div className="panel-body">
					<div className="row">
						<div className="col-lg-5">
							<TicketDetails />
							<OpenTickets />
						</div>
						<div className="col-lg-7">
							<Messages />
						</div>
					</div>
				</div>
			</div>
		);

	}

}

interface IProps {
	ticket: Ticket
	isLoading: boolean
	hasErrored: boolean
	connectedUsers: TicketUserStateModel[]
}

const mapStateToProps = (state: IAppState): IProps => ({
	ticket: state.ticket,
	isLoading: state.status.isLoading,
	hasErrored: state.status.hasErrored,
	connectedUsers: state.connectedUsers
});

export default connect(mapStateToProps)(Base);
