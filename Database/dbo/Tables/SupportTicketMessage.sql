CREATE TABLE [dbo].[SupportTicketMessage]
(
	[Id]   INT            IDENTITY (1, 1) NOT NULL,
    [TicketId] INT NOT NULL, 
	[Sender] NVARCHAR(128) NOT NULL, 
    [Message] NVARCHAR(MAX) NOT NULL, 
    [IsAdminReply] BIT NOT NULL DEFAULT 0, 
	[TimeStamp] DATETIME2 NOT NULL, 
    CONSTRAINT [PK_SupportTicketMessage] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_SupportTicketMessage_Ticket] FOREIGN KEY ([TicketId]) REFERENCES [SupportTicket]([Id])
)
