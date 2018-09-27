CREATE TABLE [dbo].[LottoTicket] (
	[Id]			INT IDENTITY (1, 1) NOT NULL,
	[UserId]		UNIQUEIDENTIFIER	NOT NULL,
	[LottoItemId]	INT					NOT NULL,
	[DrawId]		INT					NOT NULL,
	[Timestamp]		DATETIME2			NOT NULL,
	[IsArchived]	BIT					NOT NULL, 
	CONSTRAINT [PK_Lotto] PRIMARY KEY CLUSTERED ([Id] ASC), 
	CONSTRAINT [FK_Lotto_LottoItem] FOREIGN KEY ([LottoItemId]) REFERENCES [LottoItem]([Id])
);

