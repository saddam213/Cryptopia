CREATE TABLE [dbo].[IntegrationMarketData] (
	[Id]					INT				IDENTITY (1, 1) NOT NULL,
	[IntegrationExchangeId]	INT				NOT NULL,
	[TradePairId]			INT				NOT NULL,
	[Bid]					DECIMAL(38, 8)	NOT NULL,
	[Ask]					DECIMAL(38, 8)	NOT NULL,
	[Last]					DECIMAL(38, 8)	NOT NULL,
	[Volume]				DECIMAL(38, 8)	NOT NULL,
	[BaseVolume]			DECIMAL(38, 8)	NOT NULL,
	[MarketUrl]				NVARCHAR(256)	NULL,
	[Timestamp]				DATETIME2(7)	NOT NULL,
    CONSTRAINT [PK_IntegrationMarketData] PRIMARY KEY CLUSTERED ([Id] ASC), 
	CONSTRAINT [FK_IntegrationMarketData_IntegrationExchangeId] FOREIGN KEY ([IntegrationExchangeId]) REFERENCES [IntegrationExchange]([Id]),
  	CONSTRAINT [FK_IntegrationMarketData_TradePairId] FOREIGN KEY ([TradePairId]) REFERENCES [TradePair]([Id])
);

