CREATE TABLE [dbo].[MarketFeedback]
(
    [Id] INT IDENTITY (0, 1) NOT NULL, 
    [MarketItemId] INT NOT NULL,
	[SenderUserId] UNIQUEIDENTIFIER NOT NULL,
    [ReceiverUserId] UNIQUEIDENTIFIER NOT NULL, 
    [Rating] INT NOT NULL, 
    [Comment] VARCHAR(256) NOT NULL, 
    [Timestamp] DATETIME2 NOT NULL, 
    CONSTRAINT [PK_MarketFeedback] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_MarketItemImage_MarketFeedback] FOREIGN KEY ([MarketItemId]) REFERENCES [MarketItem]([Id]),
	CONSTRAINT [FK_MarketItemImage_UserSender] FOREIGN KEY ([SenderUserId]) REFERENCES [User]([Id]),
	CONSTRAINT [FK_MarketItemImage_UserReceiver] FOREIGN KEY ([ReceiverUserId]) REFERENCES [User]([Id]),
)
