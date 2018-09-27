CREATE TABLE [dbo].[TradePair] (
    [Id]              INT    IDENTITY (1, 1) NOT NULL,
    [CurrencyId1]          INT         NOT NULL,
    [CurrencyId2]        INT         NOT NULL
    CONSTRAINT [PK_TradePair] PRIMARY KEY CLUSTERED ([Id] ASC), 
    [LastTrade] DECIMAL(38, 8) NOT NULL, 
    [Change] FLOAT NOT NULL, 
    [StatusId] TINYINT NOT NULL, 
    [StatusMessage] NVARCHAR(1024) NULL, 
    CONSTRAINT [FK_TradePair_Currency1] FOREIGN KEY ([CurrencyId1]) REFERENCES [Currency]([Id]),
	CONSTRAINT [FK_TradePair_Currency2] FOREIGN KEY ([CurrencyId2]) REFERENCES [Currency]([Id])
);

