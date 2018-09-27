CREATE TABLE [dbo].[MarketItemQuestion]
(
    [Id] INT IDENTITY (0, 1) NOT NULL, 
	[UserId] UNIQUEIDENTIFIER NOT NULL, 
    [MarketItemId] INT NOT NULL,
	[Question] NVARCHAR(1024) NOT NULL,
	[Answer] NVARCHAR(1024) NOT NULL, 
    [Timestamp] DATETIME2 NOT NULL, 
    CONSTRAINT [PK_MarketItemQuestion] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_MarketItemQuestion_MarketItem] FOREIGN KEY ([MarketItemId]) REFERENCES [MarketItem]([Id]),
	CONSTRAINT [FK_MarketItemQuestion_User] FOREIGN KEY ([UserId]) REFERENCES [User]([Id])
)
