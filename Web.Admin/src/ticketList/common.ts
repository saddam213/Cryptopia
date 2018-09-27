import * as models from './store/models'

let razorJsonModel: RazorModel = null;

export function getRazorModel(): RazorModel {
	if (razorJsonModel == null) {
		var json = (window as any).razorJsonModel;
		razorJsonModel = JSON.parse(json);
	}
	return razorJsonModel;
}

export function getQueues(): models.Queue[] {
	return [
		<models.Queue>{ Id: models.SepcialQueueType.ALL, Name: models.SepcialQueueType.ALL },

		...getRazorModel().viewModel.Queues.map(q => <models.Queue>({
			Id: q.Name,
			Name: q.Name,
			EntityId: q.Id
		})),

		<models.Queue>{ Id: models.SepcialQueueType.CLOSED, Name: models.SepcialQueueType.CLOSED }
	]
}

class SupportQueueModel {
	Id: number
	Name: string
}

class TicketListViewModel {
	Queues: SupportQueueModel[]
}

export class RazorModel {
	viewModel: TicketListViewModel
	moveTicketAction		 : string
	ticketDetailsAction	 : string
	getOpenTicketsAction : string
	userAction					 : string
};


