CREATE PROCEDURE [dbo].[GetUserTradeHistoryByTradePair]
	  @UserId UNIQUEIDENTIFIER
	 ,@TradePairId INT
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT 
			o.Timestamp AS Timestamp 
			, c1.Symbol AS Symbol1 
			, c2.Symbol AS Symbol2 
			, o.Rate AS Rate 
			, o.Amount AS Amount 
			, o.Amount * o.Rate AS Total 
			, CONVERT(TINYINT,0) AS TradeType 
			, 0 AS Remaining
			, o.Id AS TradeId 
			, t.Id AS TradePairId 
			, o.Fee
	FROM [dbo].TradeHistory o
	JOIN TradeHistoryType tt ON o.TradeHistoryTypeId = tt.Id
	JOIN [dbo].[TradePair] t ON o.TradePairId = t.Id
	JOIN [dbo].[Currency] c1 ON t.CurrencyId1 = c1.Id
	JOIN [dbo].[Currency] c2 ON t.CurrencyId2 = c2.Id
	WHERE o.UserId = @UserId AND @TradePairId = o.TradePairId

	UNION ALL

	SELECT 
			o.Timestamp AS Timestamp 
			, c1.Symbol AS Symbol1 
			, c2.Symbol AS Symbol2 
			, o.Rate AS Rate 
			, o.Amount AS Amount 
			, o.Amount * o.Rate AS Total 
		    , CONVERT(TINYINT,1) AS TradeType 
			, 0 AS Remaining
			, o.Id AS TradeId 
			, t.Id AS TradePairId 
			, o.Fee
	FROM [dbo].TradeHistory o
	JOIN TradeHistoryType tt ON o.TradeHistoryTypeId = tt.Id
	JOIN [dbo].[TradePair] t ON o.TradePairId = t.Id
	JOIN [dbo].[Currency] c1 ON t.CurrencyId1 = c1.Id
	JOIN [dbo].[Currency] c2 ON t.CurrencyId2 = c2.Id
	WHERE  o.ToUserId = @UserId AND @TradePairId = o.TradePairId
	ORDER BY o.Id DESC
		
RETURN

GO


