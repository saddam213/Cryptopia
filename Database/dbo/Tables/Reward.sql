CREATE TABLE [dbo].[Reward] (
    [Id] INT    IDENTITY (1, 1) NOT NULL,
	[UserId]    UNIQUEIDENTIFIER    NOT NULL,
    [CurrencyId]  INT    NOT NULL,
    [Amount]   DECIMAL(38, 8) NOT NULL DEFAULT 0,
	[Percent]   DECIMAL(38, 8) NOT NULL DEFAULT 0,
	[RewardType]   NVARCHAR(128) NULL,
    [Message] NVARCHAR(256) NULL, 
    [TimeStamp] DATETIME2(7) NOT NULL, 
    CONSTRAINT [PK_Reward] PRIMARY KEY CLUSTERED ([Id] ASC), 
);

