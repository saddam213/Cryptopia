CREATE TABLE [dbo].[TransferHistoryType] (
    [Id]   TINYINT            IDENTITY (0, 1) NOT NULL,
    [Name] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_TransferHistoryType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

