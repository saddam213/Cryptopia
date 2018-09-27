CREATE PROCEDURE [dbo].[ApiGetMarketOrders]
	@TradePairId INT,
	@TradeType TINYINT,
	@Count INT = 100
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT TOP (@Count)
	     T.TradePairId
		,C.Symbol + '/' + C2.Symbol AS Label
		,T.Rate AS Price
		,SUM(T.Remaining) AS Volume
		,SUM(T.Remaining) * T.Rate AS Total
	FROM [Trade] T
	JOIN TradePair TP ON TP.Id = T.TradePairId
	JOIN Currency C ON C.Id = TP.CurrencyId1
	JOIN Currency C2 ON C2.Id = TP.CurrencyId2
	WHERE TP.Id = @TradePairId
	AND T.TradeTypeId = @TradeType AND T.TradeStatusId IN (0,2)
	GROUP BY C.Symbol,C2.Symbol, T.TradePairId, T.Rate
	ORDER BY 
		CASE WHEN @TradeType = 0 THEN T.Rate END DESC,
		CASE WHEN @TradeType = 1 THEN T.Rate END ASC

RETURN
GO


