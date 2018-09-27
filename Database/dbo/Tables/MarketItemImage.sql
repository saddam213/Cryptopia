CREATE TABLE [dbo].[MarketItemImage]
(
    [Id] INT IDENTITY (0, 1) NOT NULL, 
    [MarketItemId] INT NOT NULL,
	[Image] VARCHAR(256) NOT NULL,
    CONSTRAINT [PK_MarketItemImage] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_MarketItemImage_MarketItem] FOREIGN KEY ([MarketItemId]) REFERENCES [MarketItem]([Id]),
)
