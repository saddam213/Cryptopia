CREATE TABLE [dbo].[Location]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
    [ParentId] INT NOT NULL, 
    [Name] NVARCHAR(128) NOT NULL, 
	[CountryCode] NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED ([Id] ASC) ,
)
