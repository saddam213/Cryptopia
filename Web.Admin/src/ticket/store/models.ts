export class IAppState {
	ticket: Ticket
	status: AppStatus
	messages: Message[]
	connectedUsers: TicketUserStateModel[]
}

export enum SupportTicketStatus {
	New = 'New',
	UserReply = 'UserReply',
	AdminReply = 'AdminReply',
	Closed = 'Closed',
	Reopened = 'Reopened'
}

export class AppStatus {
	isLoading: boolean
	hasErrored: boolean
	isCreatingMessage: boolean
	isEditing: boolean
	messageId?: number
}

export class Ticket {
	Id: number
	UserName: string
	Email: string
	Category: string
	Title: string
	Status: SupportTicketStatus
	Queue: string
	Description: string
	LastUpdate: string
	Created: string
	Tags: string[]
	OpenTickets: SupportTicketBasicInfoModel[]
}

export class SupportTicketBasicInfoModel {
	Id: number
	Title: string
	Created: string
}

export class Message {
	Id: number
	TicketId: number
	IsInternal: boolean
	IsDraft: boolean
	Message: string
	SenderId: string
	UserName: string
	Email: string
	LastUpdate: string
	Created: string
}

export class TicketViewModel {
	Ticket: Ticket
	Messages: Message[]
}