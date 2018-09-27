CREATE TABLE [dbo].[FaucetClaim]
(
	[Id]			INT	IDENTITY (1, 1)		NOT NULL,
	[UserId]		UNIQUEIDENTIFIER		NOT NULL,
	[CurrencyId]	INT						NOT NULL, 
	[Timestamp]		DATETIME2				NOT NULL, 
	CONSTRAINT [PK_FaucetClaim] PRIMARY KEY CLUSTERED ([Id] ASC), 
	CONSTRAINT [FK_FaucetClaim_Currency] FOREIGN KEY ([CurrencyId]) REFERENCES [Currency]([Id])
)
