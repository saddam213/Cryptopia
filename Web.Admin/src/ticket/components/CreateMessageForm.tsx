import * as React from 'react';
import { Message, IAppState, Ticket } from '../store/models'
import * as moment from 'moment'
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as _ from 'lodash'
import { creatingMessage } from '../store/appStatus.ducks'
import { createMessage } from '../store/messages.thunks'

class CreateMessageForm extends React.Component<IProps & IDispatchProps, IState> {

	constructor(props, context) {
		super(props, context);
		this.state = { message: "" };
	}

	public componentDidMount() {
		const { ticket } = this.props;
		this.setState({ message: `Hi ${ticket.UserName}, \n \n` });
	}

	render() {
		const { close, createMessage, createAdminMessage } = this.props;
		const handleMessageChange = (event: React.ChangeEvent<any>) => this.setState({ message: event.target.value });

		return (
			<div>
				<div className="row">
					<div className="col-lg-6">
						<h4>Create Message</h4>
					</div>
					<div className="col-lg-2 pull-right">
						<button type="button" className="btn pull-right" onClick={close}> Close </button>
					</div>
				</div>

				<div className="form-group">
					<textarea style={{resize: "vertical"}} className="form-control" rows={12} value={this.state.message} onChange={handleMessageChange}></textarea>
					<br />
					<div className="row">
						<div className="col-lg-5">
							<button type="button" className="btn btn-success btn-block" onClick={() => createMessage(this.state.message)}> Reply </button>
						</div>
						<div className="col-lg-2"></div>
						<div className="col-lg-5">
							<button type="button" className="btn btn-info btn-block" onClick={() => createAdminMessage(this.state.message)}> Admin Note </button>
						</div>
					</div>
				</div>
			</div>
		);

	}

}

interface IState {
	message: string
}

interface IProps {
	ticket: Ticket
}

interface IDispatchProps {
	close: () => void;
	createMessage: (message: string) => void;
	createAdminMessage: (message: string) => void;
}

const mapStateToProps = (state: IAppState): IProps => ({
	ticket: state.ticket
});

const mapDispatchToProps = (dispatch: Dispatch<any>): IDispatchProps => ({
	close: () => dispatch(creatingMessage(false)),
	createMessage: (message: string) => dispatch(createMessage(message)),
	createAdminMessage: (message: string) => dispatch(createMessage(message, true)),
});

export default connect(mapStateToProps, mapDispatchToProps)(CreateMessageForm);
