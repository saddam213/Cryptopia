﻿CREATE TABLE [dbo].[MarketItemFeature]
(
    [Id] TINYINT IDENTITY (0, 1) NOT NULL, 
    [Name] NVARCHAR(128) NOT NULL,
	CONSTRAINT [PK_MarketItemFeature] PRIMARY KEY CLUSTERED ([Id] ASC),
)
