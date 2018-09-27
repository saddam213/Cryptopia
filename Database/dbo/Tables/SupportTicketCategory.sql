CREATE TABLE [dbo].[SupportTicketCategory]
(
	[Id]   TINYINT            IDENTITY (0, 1) NOT NULL,
    [Name] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_SupportTicketCategory] PRIMARY KEY CLUSTERED ([Id] ASC)
)
