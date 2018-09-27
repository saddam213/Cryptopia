CREATE TABLE [dbo].[Withdraw] (
    [Id]				INT    IDENTITY (1, 1)	NOT NULL,
    [UserId]			UNIQUEIDENTIFIER        NOT NULL,
    [CurrencyId]		INT						NOT NULL,
	[Address]			NVARCHAR(256)			NOT NULL,
    [Amount]			DECIMAL(38, 8)			NOT NULL,
	[Fee]				DECIMAL(38, 8)			NOT NULL,
    [TxId]				NVARCHAR(256)			NOT NULL,
    [Confirmations]		INT						NOT NULL, 
	[WithdrawTypeId]	TINYINT					NOT NULL, 
	[WithdrawStatusId]	TINYINT					NOT NULL, 
	[TwoFactorToken]	NVARCHAR(512)			NOT NULL,
	[IsApi]				BIT						NOT NULL,
	[TimeStamp]			DATETIME2				NOT NULL, 
    CONSTRAINT [PK_Withdraw] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Withdraw_Currency] FOREIGN KEY ([CurrencyId]) REFERENCES [Currency]([Id]), 
    CONSTRAINT [FK_Withdraw_WithdrawType] FOREIGN KEY ([WithdrawTypeId]) REFERENCES [WithdrawType]([Id]),
	CONSTRAINT [FK_Withdraw_WithdrawStatus] FOREIGN KEY ([WithdrawStatusId]) REFERENCES [WithdrawStatus]([Id])
);

