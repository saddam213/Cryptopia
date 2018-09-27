CREATE TABLE [dbo].[TradeHistoryType] (
    [Id]   TINYINT            IDENTITY (0, 1) NOT NULL,
    [Name] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_TradeHistoryType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

