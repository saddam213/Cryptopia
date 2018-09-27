﻿CREATE TABLE [dbo].[ErrorLog] (
    [Id]        INT  IDENTITY (1, 1) NOT NULL, 
	[Component] NVARCHAR(128)	NOT NULL,
	[Method]	NVARCHAR(128)	NOT NULL,
	[Request]	NVARCHAR(MAX)	NOT NULL,
    [Exception] NVARCHAR(MAX)	NOT NULL,
	[Timestamp] DATETIME2		NOT NULL,
    CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED ([Id] ASC)
);

