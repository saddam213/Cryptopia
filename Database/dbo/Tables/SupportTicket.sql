CREATE TABLE [dbo].[SupportTicket]
(
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL, 
    [Title] NVARCHAR(512) NOT NULL, 
    [Description] NVARCHAR(MAX) NOT NULL, 
    [StatusId] TINYINT NOT NULL, 
    [CategoryId] TINYINT NOT NULL, 
    [LastUpdate] DATETIME2 NOT NULL, 
	[Created] DATETIME2 NOT NULL, 
    CONSTRAINT [PK_SupportTicket] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_SupportTicket_Status] FOREIGN KEY ([StatusId]) REFERENCES [SupportTicketStatus]([Id]),
	CONSTRAINT [FK_SupportTicket_Category] FOREIGN KEY ([CategoryId]) REFERENCES [SupportTicketCategory]([Id])
)
