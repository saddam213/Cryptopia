CREATE PROCEDURE [dbo].[GetTradeHistoryByTradePair]
	 @TradePairId INT
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT TOP 500 
		 c1.Symbol
		,c2.Symbol 
		,o.Rate 
		,o.Amount 
		,o.Amount * o.Rate AS Total
		,o.TradeHistoryTypeId  
		,o.Timestamp
	FROM [dbo].TradeHistory o
	JOIN [dbo].[TradePair] t ON o.TradePairId = t.Id
	JOIN [dbo].[Currency] c1 ON t.CurrencyId1 = c1.Id
	JOIN [dbo].[Currency] c2 ON t.CurrencyId2 = c2.Id
	WHERE t.Id = @TradePairId
	ORDER BY Timestamp DESC
	
RETURN
GO


