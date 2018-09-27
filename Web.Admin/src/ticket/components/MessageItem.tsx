import * as React from 'react';
import { Message, Ticket, SupportTicketStatus, IAppState, AppStatus } from '../store/models'
import { isModifingState } from '../store/models.helpers'
import * as moment from 'moment'
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { getRazorModel } from '../common';
import { editing } from '../store/appStatus.ducks'
import * as messageThunks from '../store/messages.thunks'
import EditMessageForm from './EditMessageForm'


class MessageItem extends React.Component<IProps & IStoreProps & IDispatchProps> {

	render() {
		const { message, appStatus } = this.props;

		const backgroundColor = message.IsDraft
			? "#f6f6f6"
			: message.IsInternal
				? "#e6f3f7"
				: "";

		let isEditingMessage: boolean = appStatus.isEditing && appStatus.messageId == message.Id;

		let content = isEditingMessage ? <EditMessageForm /> : this.messageContent;

		return (
			<div className="list-group-item list-group-item-action flex-column" style={{ backgroundColor }}>
				{content}
			</div>
		)
	}

	get messageContent() {
		const { message } = this.props;

		return (
			<div className="row">
				<div className="col-lg-11">
					{this.titleSection}
					<pre>{message.Message}</pre>
				</div>
				<div className="col-lg-1">
					{this.buttonGroup}
				</div>
			</div>
		)
	}

	get buttonGroup() {
		const { userId } = getRazorModel();
		const { message, ticket, publishMessage, editMessage, deleteMessage, appStatus } = this.props;

		if ((isModifingState(appStatus)) || ticket.Status == SupportTicketStatus.Closed || message.SenderId != userId) {
			return <div />;
		}

		var btns: JSX.Element[] = [];


		if (message.IsInternal || message.IsDraft) {
			btns.push(
				<button className="btn btn-info" title="Edit" key="edit" onClick={() => editMessage(message.Id)}>
					<span className="fa fa-pencil-square-o"></span>
				</button>);
		}

		if (message.IsDraft) {
			btns.push(
				<button className="btn btn-success" title="Publish" key="publish" onClick={() => publishMessage(message.Id)}>
					<span className="fa fa-check"></span>
				</button>);

			btns.push(<button className="btn btn-danger" title="Delete" key="delete" onClick={() => deleteMessage(message.Id)}>
				<span className="fa fa-trash-o"></span>
			</button>);
		}

		return (
			<div className="btn-group-vertical" role="group">
				{ btns }
			</div>
			)
	}

	get titleSection() {
		const { message } = this.props;

		let title = message.IsDraft
			? `${message.UserName} - Draft Message`
			: message.IsInternal
			? `${message.UserName} - Admin Note`
			: message.UserName;

		return (
			<div className="row">
				<div className="col-lg-6">
					<h5>Sender: {title}</h5>
				</div>
				<div className="col-lg-6">
					<h5 className="pull-right">{moment(message.LastUpdate).format('MMM Do YYYY, hh:mm')}</h5>
				</div>
			</div>)
	}

}

interface IProps {
	message: Message
}

interface IStoreProps {
	ticket: Ticket
	appStatus: AppStatus
}

interface IDispatchProps {
	deleteMessage: (id: number) => void
	publishMessage: (id: number) => void
	editMessage: (messageId: number) => void
}

const mapStateToProps = (state: IAppState, props: IProps): IProps & IStoreProps => {

	return {
		...props,
		ticket: state.ticket,
		appStatus: state.status
	}
}

const mapDispatchToProps = (dispatch: Dispatch<any>): IDispatchProps => ({
	deleteMessage: (id) => dispatch(messageThunks.deleteMessage(id)),
	publishMessage: (id) => dispatch(messageThunks.publishMessage(id)),
	editMessage: (messageId: number) => dispatch(editing(true, messageId)),
});

export default connect(mapStateToProps, mapDispatchToProps)(MessageItem);
