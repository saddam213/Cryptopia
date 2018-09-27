CREATE TABLE [dbo].[Trade]
(
	[Id]   INT            IDENTITY (1, 1) NOT NULL, 
    [UserId] UNIQUEIDENTIFIER NOT NULL, 
    [TradeTypeId] TINYINT NOT NULL,
	[TradePairId] INT NOT NULL,  
    [Rate] DECIMAL(38, 8) NOT NULL, 
    [Amount] DECIMAL(38, 8) NOT NULL, 
    [Remaining] DECIMAL(38, 8) NOT NULL,
	[Fee] DECIMAL(38, 8) NOT NULL,
	[TradeStatusId] TINYINT NOT NULL, 
	[IsApi] BIT NOT NULL, 
	[Timestamp] DATETIME2 NOT NULL,
    CONSTRAINT [PK_Trade] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Trade_TradePair] FOREIGN KEY ([TradePairId]) REFERENCES [TradePair]([Id]),
	CONSTRAINT [FK_Trade_TradeHistoryType] FOREIGN KEY ([TradeTypeId]) REFERENCES [TradeHistoryType]([Id]),
	CONSTRAINT [FK_Trade_TradeStatus] FOREIGN KEY ([TradeStatusId]) REFERENCES [TradeStatus]([Id])
)
