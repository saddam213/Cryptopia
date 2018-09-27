import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as _ from 'lodash'

import { Queue, Ticket, IAppState } from '../store/models'
import { moveTicketAndUpdate } from '../store/ticketTable.ducks'

class QueueSelector extends React.Component<QueueSelectorProps, {}> {

	render() {

		const { queues, ticket, tableId, moveTicket } = this.props;

		const ticketQueueName = _.find(queues, (q) => q.EntityId == ticket.QueueId).Name;

		const onQueueSelected = (queue: Queue) => (e) => {
			moveTicket(ticket, tableId, queue.EntityId);
		}

		const liElements: JSX.Element[] = queues.map((queue: Queue) => {
			if (queue.EntityId != null && queue.EntityId != undefined) {
				var isSelected = ticket.QueueId === queue.EntityId;
				return <li
					className={isSelected ? "selected" : ""}
					onClick={onQueueSelected(queue)}
					key={queue.Id}
				>{queue.Name}</li>;
			}
		});

		return (
			<a className="btn btn-primary btn-select btn-select-light" style={{margin: 0}}>
				<input type="hidden" className="btn-select-input" id="" name="" value="" />
				<span className="btn-select-value">{ticketQueueName}</span>
				<span className='btn-select-arrow glyphicon glyphicon-chevron-down'></span>
				<ul>
					{ liElements }
				</ul>
			</a>)

	}

	public componentDidMount() {
		var $this = $(ReactDOM.findDOMNode(this));

		$this.on('click', function (e) {
			var ul = $(this).find("ul");
			if ($(this).hasClass("active")) {
				//if (ul.find("li").is(e.target)) {
				//	$(e.target).addClass("selected").siblings().removeClass("selected");
				//}
				ul.hide();
				$(this).removeClass("active");
			}
			else {
				$('.btn-select').not(this).each(function () {
					$(this).removeClass("active").find("ul").hide();
				});
				ul.slideDown(300);
				$(this).addClass("active");
			}
		});

		$(document).on('click', function (e) {
			var target = $(e.target).closest(".btn-select");
			if (!target.length) {
				$(".btn-select").removeClass("active").find("ul").hide();
			}
		});
	}
}

interface IProps {
	queues: Queue[];
}

interface IDispatchProps {
	moveTicket: (ticketId: Ticket, tableId: string, toQueueId: number) => void
}

interface IComponentProps {
	ticket: Ticket;
	tableId: string;
}

export type QueueSelectorProps = IProps & IDispatchProps & IComponentProps

const mapDispatchToProps = (dispatch: Dispatch<any>): IDispatchProps => ({
	moveTicket: (ticket, tableId, toQueueId) => dispatch(moveTicketAndUpdate(ticket, tableId, toQueueId))
});

const mapStateToProps = (state: IAppState, props: IComponentProps): IProps & IComponentProps => ({
	...props,
	queues: state.queues,
});

export default connect(mapStateToProps, mapDispatchToProps)(QueueSelector);