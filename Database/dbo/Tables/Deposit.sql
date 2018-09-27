CREATE TABLE [dbo].[Deposit] (
    [Id]				INT    IDENTITY (1, 1)	NOT NULL,
    [UserId]			UNIQUEIDENTIFIER        NOT NULL,
    [CurrencyId]		INT						NOT NULL,
    [Amount]			DECIMAL(38, 8)			NOT NULL,
    [TxId]				NVARCHAR(256)			NOT NULL,
    [Confirmations]		INT						NOT NULL, 
	[DepositTypeId]		TINYINT					NOT NULL, 
	[DepositStatusId]		TINYINT					NOT NULL,
	[TimeStamp]			DATETIME2				NOT NULL, 
    CONSTRAINT [PK_Deposit] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Deposit_Currency] FOREIGN KEY ([CurrencyId]) REFERENCES [Currency]([Id]), 
    CONSTRAINT [FK_Deposit_DepositType] FOREIGN KEY ([DepositTypeId]) REFERENCES [DepositType]([Id]),
	CONSTRAINT [FK_Deposit_DepositStatus] FOREIGN KEY ([DepositStatusId]) REFERENCES [DepositStatus]([Id])
);

