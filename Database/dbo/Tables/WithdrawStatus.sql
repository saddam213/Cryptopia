CREATE TABLE [dbo].[WithdrawStatus] (
    [Id]   TINYINT            IDENTITY (0, 1) NOT NULL,
    [Name] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_WithdrawStatus] PRIMARY KEY CLUSTERED ([Id] ASC)
);

