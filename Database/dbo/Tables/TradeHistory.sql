CREATE TABLE [dbo].[TradeHistory] (
    [Id]              INT    IDENTITY (1, 1) NOT NULL,
    [UserId]          UNIQUEIDENTIFIER         NOT NULL,
    [ToUserId]        UNIQUEIDENTIFIER         NOT NULL,
    [TradePairId]         INT  NOT NULL,
	[CurrencyId]         INT  NOT NULL,
    [TradeHistoryTypeId]    TINYINT        NOT NULL,
    [Amount] DECIMAL(38, 8)	 NOT NULL, 
	[Rate] DECIMAL(38, 8)	 NOT NULL, 
	[Fee] DECIMAL(38, 8)	 NOT NULL, 
	[IsApi] BIT	 NOT NULL, 
	[Timestamp] DATETIME2 NOT NULL, 
    CONSTRAINT [PK_TradeHistory] PRIMARY KEY CLUSTERED ([Id] ASC), 
	CONSTRAINT [FK_TradeHistory_TradePair] FOREIGN KEY ([TradePairId]) REFERENCES [TradePair]([Id]),
	CONSTRAINT [FK_TradeHistory_Currency] FOREIGN KEY ([CurrencyId]) REFERENCES [Currency]([Id]),
    CONSTRAINT [FK_TradeHistory_TransactionType] FOREIGN KEY ([TradeHistoryTypeId]) REFERENCES [TradeHistoryType]([Id])
);

