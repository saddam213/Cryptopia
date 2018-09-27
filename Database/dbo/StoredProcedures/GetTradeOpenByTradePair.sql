CREATE PROCEDURE [dbo].[GetTradeOpenByTradePair]
	@TradePairId INT
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT 
		 C.Symbol AS Symbol1
		,T.Rate
		,SUM(T.Remaining) AS Amount
		,SUM(T.Remaining) * T.Rate AS Total
		,T.TradeTypeId AS TradeType
	FROM [Trade] T
	JOIN TradePair TP ON TP.Id = T.TradePairId
	JOIN Currency C ON C.Id = TP.CurrencyId1
	WHERE TP.Id = @TradePairId AND T.TradeStatusId IN (0,2)
	GROUP BY T.TradeTypeId, C.Symbol, T.Rate

RETURN
GO
GO


