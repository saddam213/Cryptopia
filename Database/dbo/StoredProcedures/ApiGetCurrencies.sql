CREATE PROCEDURE [dbo].[ApiGetCurrencies]
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT
	 c.Id,
	 c.Symbol,
	 c.Name,
	 a.Name AS [Algorithm]
	FROM Currency c
	JOIN AlgoType a on a.Id = c.AlgoTypeId
    
RETURN

GO



