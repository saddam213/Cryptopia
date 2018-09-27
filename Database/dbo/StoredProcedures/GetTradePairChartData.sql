CREATE PROCEDURE [dbo].[GetTradePairChartData]
  @TradePairId INT
, @Interval INT = 1 --hour

AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    SELECT 
	o.Amount,
	o.Rate,
	o.Timestamp
	FROM dbo.TradeHistory o 
	WHERE o.TradePairId = @TradePairId 
	AND o.TradeHistoryTypeId in (0,1) -- buy, sell
	AND o.Timestamp >= DATEADD(HOUR, -@Interval, GETDATE())
	ORDER BY o.Timestamp DESC

RETURN
GO


