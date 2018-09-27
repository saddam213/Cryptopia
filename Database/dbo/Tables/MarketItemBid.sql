CREATE TABLE [dbo].[MarketItemBid]
(
    [Id] INT IDENTITY (0, 1) NOT NULL, 
	[UserId] UNIQUEIDENTIFIER NOT NULL, 
    [MarketItemId] INT NOT NULL,
	[BidAmount] DECIMAL(38, 8) NOT NULL,
	 [IsWinningBid] BIT NOT NULL, 
	[Timestamp] DATETIME2 NOT NULL, 
    CONSTRAINT [PK_MarketItemBid] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_MarketItemBid_MarketItem] FOREIGN KEY ([MarketItemId]) REFERENCES [MarketItem]([Id]),
)
