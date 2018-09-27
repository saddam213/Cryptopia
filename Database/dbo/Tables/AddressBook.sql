CREATE TABLE [dbo].[AddressBook] (
	[Id]			INT IDENTITY (1, 1) NOT NULL,
	[UserId]		UNIQUEIDENTIFIER	NOT NULL,
	[CurrencyId]	INT					NOT NULL,
	[Label]			VARCHAR(128)		NOT NULL,
	[Address]		NVARCHAR(128)		NOT NULL,
	[IsEnabled]		BIT					NOT NULL,
	CONSTRAINT [PK_AddressBook] PRIMARY KEY CLUSTERED ([Id] ASC), 
	CONSTRAINT [FK_AddressBook_Currency] FOREIGN KEY ([CurrencyId]) REFERENCES [Currency]([Id])
);

