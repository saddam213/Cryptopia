import * as models from './store/models'

let razorJsonModel: RazorModel = null;

export function getRazorModel(): RazorModel {
	if (razorJsonModel == null) {
		var json = (window as any).razorJsonModel;
		razorJsonModel = JSON.parse(json);
	}
	return razorJsonModel;
}

export class RazorModel {
	ticketViewModel: models.TicketViewModel
	userId: string
	userName: string
	closeTicketAction: string
	reopenTicketAction: string
	getTicketDetailsAction: string
	createMessageAction : string
	deleteMessageAction: string
	editMessageAction: string
	publishMessageAction: string
	updateTicketAction: string
	userAction: string
	ticketDetailsAction: string
};
