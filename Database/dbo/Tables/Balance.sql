CREATE TABLE [dbo].[Balance] (
    [Id] INT    IDENTITY (1, 1) NOT NULL,
	[UserId]    UNIQUEIDENTIFIER    NOT NULL,
    [CurrencyId]  INT    NOT NULL,
    [Total]   DECIMAL(38, 8) NOT NULL DEFAULT 0,
	[Unconfirmed]   DECIMAL(38, 8) NOT NULL DEFAULT 0,
    [HeldForTrades] DECIMAL(38, 8) NOT NULL DEFAULT 0, 
    [PendingWithdraw] DECIMAL(38, 8) NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_Balance] PRIMARY KEY CLUSTERED ([Id] ASC), 
);

