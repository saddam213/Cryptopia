﻿CREATE TABLE [dbo].[AlgoType]
(
    [Id] TINYINT IDENTITY (0, 1) NOT NULL, 
    [Name] NVARCHAR(128) NOT NULL,
	CONSTRAINT [PK_AlgoType] PRIMARY KEY CLUSTERED ([Id] ASC),
)
