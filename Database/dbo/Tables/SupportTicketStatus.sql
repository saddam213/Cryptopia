﻿CREATE TABLE [dbo].[SupportTicketStatus]
(
	[Id]   TINYINT            IDENTITY (0, 1) NOT NULL,
    [Name] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_SupportTicketStatus] PRIMARY KEY CLUSTERED ([Id] ASC)
)
