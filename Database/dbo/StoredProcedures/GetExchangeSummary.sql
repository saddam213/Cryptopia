CREATE PROCEDURE [dbo].[GetExchangeSummary]
	@Hours INT = 24
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	DECLARE @Processing TABLE
	(
	  TradePairId INT,
	  TotalTrades INT,
	  Low DECIMAL(38,8),
	  High  DECIMAL(38,8),
	  Volume DECIMAL(38,8),
	  Symbol NVARCHAR(50),
	  BaseSymbol NVARCHAR(50),
	  TotalBase DECIMAL(38,8)
	)

	INSERT INTO @Processing
	SELECT 
		TradePairId,
		COUNT(1),
		MIN(Rate),
		Max(Rate),
		SUM(Amount),
		MAX(c1.Symbol),
		MAX(c2.Symbol),
		SUM(Amount * Rate)  
	FROM TradeHistory t WITH (NOLOCK)
	JOIN TradePair tp WITH (NOLOCK) ON tp.Id = t.TradePairId
	JOIN Currency c1 WITH (NOLOCK) ON c1.Id = tp.CurrencyId1
	JOIN Currency c2 WITH (NOLOCK) ON c2.Id = tp.CurrencyId2
	WHERE Timestamp > DATEADD(HOUR,-@Hours, GETDATE())
	GROUP BY TradePairId

	SELECT * FROM @Processing
	ORDER BY TotalTrades DESC
	
RETURN
GO


