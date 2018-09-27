CREATE TABLE [dbo].[MarketCategory]
(
    [Id] INT IDENTITY (0, 1) NOT NULL, 
	[ParentId] INT NOT NULL, 
    [Name] NVARCHAR(128) NOT NULL,
	[DisplayName] NVARCHAR(128) NOT NULL,
	CONSTRAINT [PK_MarketCategory] PRIMARY KEY CLUSTERED ([Id] ASC),
)
