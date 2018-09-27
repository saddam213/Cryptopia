CREATE TABLE [dbo].[LottoHistory] (
	[Id]			INT	IDENTITY (1, 1)	NOT NULL,
	[UserId]		UNIQUEIDENTIFIER	NOT NULL,
	[LottoItemId]	INT					NOT NULL,
	[LottoTicketId]	INT					NOT NULL,
	[LottoDrawId]	INT					NOT NULL,
	[Position]		INT					NOT NULL DEFAULT(0),
	[Amount]		DECIMAL(38,8)		NOT NULL DEFAULT(0),
	[Percent]		DECIMAL(38,8)		NOT NULL DEFAULT(0), 
	[Timestamp]		DATETIME2			NOT NULL, 
	CONSTRAINT [PK_LottoHistory] PRIMARY KEY CLUSTERED ([Id] ASC), 
	CONSTRAINT [FK_LottoHistory_LottoItemId] FOREIGN KEY ([LottoItemId]) REFERENCES [LottoItem]([Id]),
	CONSTRAINT [FK_LottoHistory_LottoTickedId] FOREIGN KEY ([LottoTicketId]) REFERENCES [LottoTicket]([Id])
);

