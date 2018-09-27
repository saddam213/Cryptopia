import * as React from 'react';
import { Message, Ticket, SupportTicketStatus, IAppState, AppStatus } from '../store/models'
import { isModifingState } from '../store/models.helpers'

import * as moment from 'moment'
import { Dispatch } from 'redux';
import { connect } from 'react-redux';

import MessageItem from './MessageItem';
import { creatingMessage } from '../store/appStatus.ducks'
import CreateMessageForm from './CreateMessageForm'

interface IProps {
	messages: Message[]
	ticket: Ticket
	appStatus: AppStatus
}

interface IDispatchProps {
	createMessage: () => void;
}

class Messages extends React.Component<IProps & IDispatchProps> {

	render() {

		const { messages } = this.props;

		return (
			<div className="container-fluid">

				<div className="list-group">
					{this.replyComponent}
					{messages.sort((a, b): number => {
						return a.Id > b.Id ? -1 : 1;
					}).map((m, i) => <MessageItem message={m} key={i} />)}
				</div>
			</div>
		)
	}

	get createMessageBtn() {
		const { createMessage } = this.props;
		return  <button className="btn btn-info btn-block" onClick={createMessage}>Create Message</button>;
	}

	get replyComponent() {
		const { ticket, appStatus } = this.props;
		if (ticket.Status == SupportTicketStatus.Closed || (isModifingState(appStatus) && !appStatus.isCreatingMessage)) {
			return <div />;
		}

		var content = appStatus.isCreatingMessage
			? (
				<CreateMessageForm/>
			)
			: (
				<div className="row">
					<div className="col-lg-4"></div>
					<div className="col-lg-4">{this.createMessageBtn}</div>
					<div className="col-lg-4"></div>
				</div >
			);


		return (
			<div className="list-group-item list-group-item-action flex-column">
				{content}
			</div>
		);
	}

}

const mapStateToProps = (state: IAppState): IProps => ({
	messages: state.messages,
	ticket: state.ticket,
	appStatus: state.status
});

const mapDispatchToProps = (dispatch: Dispatch<any>): IDispatchProps => ({
	createMessage: () => dispatch(creatingMessage(true))
});

export default connect(mapStateToProps, mapDispatchToProps)(Messages);
