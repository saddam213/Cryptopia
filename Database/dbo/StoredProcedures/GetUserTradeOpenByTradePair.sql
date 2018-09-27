CREATE PROCEDURE [dbo].[GetUserTradeOpenByTradePair]
	 @UserId uniqueidentifier
	,@TradePairId int
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT 
			o.Id AS TradeId 
			, c1.Symbol AS Symbol1 
			, c2.Symbol AS Symbol2 
			, o.Rate AS Rate 
			, o.Amount AS Amount 
			, o.Amount * o.Rate AS Total 
			, o.TradeTypeId AS TradeType 
			, o.Timestamp AS Timestamp 
			, o.Remaining AS Remaining 
			, t.Id AS TradePairId 
	FROM [dbo].[Trade] o
	JOIN TradeHistoryType tt ON o.TradeTypeId = tt.Id
	JOIN [dbo].[TradePair] t ON o.TradePairId = t.Id
	JOIN [dbo].[Currency] c1 ON t.CurrencyId1 = c1.Id
	JOIN [dbo].[Currency] c2 ON t.CurrencyId2 = c2.Id
	WHERE o.UserId = @UserId 
	AND t.Id = @TradePairId 
	AND o.TradeStatusId IN (0, 2)
	ORDER BY Timestamp DESC

RETURN

GO


