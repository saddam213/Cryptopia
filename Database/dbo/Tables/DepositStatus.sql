CREATE TABLE [dbo].[DepositStatus] (
    [Id]   TINYINT            IDENTITY (0, 1) NOT NULL,
    [Name] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_DepositStatus] PRIMARY KEY CLUSTERED ([Id] ASC)
);

