CREATE TABLE [dbo].[IntegrationExchange] (
	[Id]			INT    IDENTITY (1, 1)	NOT NULL,
	[Name]			NVARCHAR(50)			NOT NULL,
	[Order]			INT						NOT NULL,
	[IsEnabled]		BIT						NOT NULL,
	CONSTRAINT [PK_IntegrationExchange] PRIMARY KEY CLUSTERED ([Id] ASC)
);

