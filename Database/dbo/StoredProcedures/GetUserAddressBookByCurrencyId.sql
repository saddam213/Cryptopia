CREATE PROCEDURE [dbo].[GetUserAddressBookByCurrencyId]
	@UserId UNIQUEIDENTIFIER
	,@CurrencyId INT
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    SELECT 
		 a.Id
		,a.CurrencyId
		,a.Label 
		,a.Address
		,c.Symbol
	FROM [AddressBook] a WITH(NOLOCK)
	JOIN Currency c on c.Id = a.CurrencyId
	WHERE a.CurrencyId = @CurrencyId AND a.IsEnabled = 1
	ORDER BY a.Label

RETURN
GO


