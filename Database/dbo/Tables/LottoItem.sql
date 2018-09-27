CREATE TABLE [dbo].[LottoItem] (
	[Id]			INT	IDENTITY (1, 1)	NOT NULL,
	[Name]			NVARCHAR(128)		NULL,
	[Description]	NVARCHAR(4000)		NULL,
	[CurrencyId]	INT					NOT NULL,
	[Prizes]		INT					NOT NULL DEFAULT(3),
	[Rate]			DECIMAL(38,8)		NOT NULL DEFAULT(0), 
	[Fee]			DECIMAL(38,8)		NOT NULL DEFAULT(0), 
	[CharityFee]	DECIMAL(38,8)		NOT NULL DEFAULT(0), 
	[Hours]			INT					NOT NULL DEFAULT(24), 
	[LottoType]		TINYINT				NOT NULL DEFAULT(0),
	[StartDate]		DATETIME2			NOT NULL, 
	[NextDraw]		DATETIME2			NOT NULL, 
	[CurrentDrawId]	INT					NOT NULL DEFAULT(0), 
	[IsDisabled]	BIT					NOT NULL, 
	[IsEnabled]		BIT					NOT NULL, 
	CONSTRAINT [PK_LottoItem] PRIMARY KEY CLUSTERED ([Id] ASC), 
	CONSTRAINT [FK_Lotto_CurrencyId] FOREIGN KEY ([CurrencyId]) REFERENCES [Currency]([Id])
);

