CREATE TABLE [dbo].[TransferHistory] (
    [Id]              INT    IDENTITY (1, 1) NOT NULL,
    [UserId]          UNIQUEIDENTIFIER         NOT NULL,
    [ToUserId]        UNIQUEIDENTIFIER         NOT NULL,
	[CurrencyId]         INT  NOT NULL,
    [TransferType]    TINYINT        NOT NULL,
    [Amount] DECIMAL(38, 8)	 NOT NULL, 
	[Fee] DECIMAL(38, 8)	 NOT NULL, 
	[IsApi] BIT NOT NULL, 
	[Timestamp] DATETIME2 NOT NULL, 
    CONSTRAINT [PK_TransferHistory] PRIMARY KEY CLUSTERED ([Id] ASC), 
	CONSTRAINT [FK_TransferHistory_Currency] FOREIGN KEY ([CurrencyId]) REFERENCES [Currency]([Id])
);

