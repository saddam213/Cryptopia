CREATE TABLE [dbo].[TradeStatus] (
    [Id]   TINYINT            IDENTITY (0, 1) NOT NULL,
    [Name] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_TradeStatus] PRIMARY KEY CLUSTERED ([Id] ASC)
);

