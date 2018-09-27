import * as React from 'react';
import { Message, IAppState } from '../store/models'
import * as moment from 'moment'
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as _ from 'lodash'
import { editing } from '../store/appStatus.ducks'
import { createMessage, editMessage } from '../store/messages.thunks'

class EditMessageForm extends React.Component<IProps & IDispatchProps, IState> {

	constructor(props, context) {
		super(props, context);
		this.state = { message: props.message.Message };
	}

	render() {
		const { close, editMessage, message } = this.props;
		const handleMessageChange = (event: React.ChangeEvent<any>) => this.setState({ message: event.target.value });

		let title = message.IsInternal ? "Edit Note" : "Edit Message";

		return (
			<div>
				<div className="row">
					<div className="col-lg-6">
						<h4>{title}</h4>
					</div>
					<div className="col-lg-2 pull-right">
						<button type="button" className="btn pull-right" onClick={close}> Close </button>
					</div>
				</div>

				<div className="form-group">
					<textarea style={{resize: "vertical"}} className="form-control"  rows={12} value={this.state.message} onChange={handleMessageChange}></textarea>
					<br />
					<div className="row">
						<div className="col-lg-5">
						</div>
						<div className="col-lg-2"></div>
						<div className="col-lg-5">
							<button type="button" className="btn btn-success btn-block" onClick={() => editMessage(message.Id, this.state.message)}> Edit </button>
						</div>
					</div>
				</div>
			</div>
		);

	}

}

interface IProps {
	message: Message;
}

interface IState {
	message: string
}

interface IDispatchProps {
	close: () => void;
	editMessage: (id: number, message: string) => void;
}

const mapStateToProps = (state: IAppState): IProps => {
	return {
		message: _.find(state.messages, (m) => m.Id == state.status.messageId)
	}
};

const mapDispatchToProps = (dispatch: Dispatch<any>): IDispatchProps => ({
	close: () => dispatch(editing(false)),
	editMessage: (id: number, message: string) => dispatch(editMessage(id, message))
});

export default connect(mapStateToProps, mapDispatchToProps)(EditMessageForm);
