CREATE PROCEDURE [dbo].[GetUserAddressBook]
	@UserId UNIQUEIDENTIFIER
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    SELECT 
		 a.Id
		,a.CurrencyId
		,c.Symbol
		,a.Label 
		,a.Address
	FROM [AddressBook] a WITH(NOLOCK)
	JOIN Currency c on c.Id = a.CurrencyId
	WHERE a.IsEnabled = 1
	ORDER BY a.Label

RETURN
GO


