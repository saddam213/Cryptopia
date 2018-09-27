CREATE TABLE [dbo].[Address] (
    [Id]              INT    IDENTITY (1, 1) NOT NULL,
    [UserId]          UNIQUEIDENTIFIER         NOT NULL,
    [CurrencyId]        INT         NOT NULL,
    [Address]         NVARCHAR(128)  NOT NULL,
	[PrivateKey]         NVARCHAR(128)  NOT NULL,
    CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([Id] ASC), 
  	CONSTRAINT [FK_Address_Currency] FOREIGN KEY ([CurrencyId]) REFERENCES [Currency]([Id])
);

