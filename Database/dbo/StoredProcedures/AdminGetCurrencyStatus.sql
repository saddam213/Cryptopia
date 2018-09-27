CREATE PROCEDURE [dbo].[AdminGetCurrencyStatus]
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT 
		c.Name,
		c.Symbol,
		c.Status,
		c.StatusMessage,
		c.Id as CurrencyId,
		c.IsFeatured
	FROM Currency c WITH(NOLOCK)

RETURN