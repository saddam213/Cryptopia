CREATE TABLE [dbo].[WithdrawType] (
    [Id]   TINYINT            IDENTITY (0, 1) NOT NULL,
    [Name] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_WithdrawType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

