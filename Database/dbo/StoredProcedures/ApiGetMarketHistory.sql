CREATE PROCEDURE [dbo].[ApiGetMarketHistory]
	 @TradePairId INT,
	 @Count INT = 1000
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
		
    SELECT TOP (@Count) 
	     o.TradePairId AS TradePairId
		,c1.Symbol + '/' + c2.Symbol AS Label
		,tt.Name AS Type
		,o.Rate AS Price
		,o.Amount AS Amount
		,CAST(o.Amount * o.Rate AS DECIMAL(38,8)) AS Total
		,dbo.UNIX_TIMESTAMP(o.Timestamp) AS Timestamp
	FROM [dbo].TradeHistory o
	JOIN [dbo].[TradePair] t ON o.TradePairId = t.Id
	JOIN [dbo].[Currency] c1 ON t.CurrencyId1 = c1.Id
	JOIN [dbo].[Currency] c2 ON t.CurrencyId2 = c2.Id
	JOIN [dbo].[TradeHistoryType] tt on tt.Id = o.TradeHistoryTypeId
	WHERE t.Id = @TradePairId
	ORDER BY Timestamp DESC
    
RETURN

GO



