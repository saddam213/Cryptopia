import { IDataTableHashMap } from '../../Reactopia/DataTable'

export enum SepcialQueueType {
	CLOSED = 'Closed',
	ALL = 'All'
}

export class IAppState {
	ticketDataTables: IDataTableHashMap
	queues: Queue[]
};

export class Ticket {
	Id: number
	UserName: string
	Category: string
	Title: string
	Status: string
	LastUpdate: string
	Opened: string
	Tags: string[]
	QueueId: number
	QueueName: number
}

export class Queue {
	Id: string
	Name: string
	EntityId?: number
}
