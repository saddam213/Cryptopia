CREATE TABLE [dbo].[DepositType] (
    [Id]   TINYINT            IDENTITY (0, 1) NOT NULL,
    [Name] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_DepositType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

