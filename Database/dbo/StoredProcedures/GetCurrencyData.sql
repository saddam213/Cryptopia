CREATE PROCEDURE [dbo].[GetCurrencyData]
@BaseCurrencyId INT = 1
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	CREATE TABLE #temp
	(
	  Tid INT,
	  Cid INT,
	  Name NVARCHAR(50),
	  Symbol NVARCHAR(50)
	)

	CREATE TABLE #stats
	(
	  Cid INT,
	  Volume DECIMAL(38,8)DEFAULT(0),
	  BaseVolume DECIMAL(38,8)DEFAULT(0),
	  High DECIMAL(38,8) DEFAULT(0),
	  Low DECIMAL(38,8)DEFAULT(0)
	)

    INSERT INTO #temp (Tid,Cid, Name, Symbol)
	SELECT tp.Id, c.Id, c.Name, c.Symbol
	FROM Currency c
	JOIN TradePair tp ON tp.CurrencyId1 = c.Id AND tp.CurrencyId2 = @BaseCurrencyId
	ORDER BY c.Id

	-- Get all the stats for the currencies
	INSERT INTO #stats (Cid, Volume,BaseVolume, High, Low)
	SELECT
		c.Id,
		ISNULL(SUM(tr.Amount ),0) + ISNULL(SUM(tr2.Amount  * tr2.Rate), 0),
		ISNULL(SUM(tr2.Amount  * tr2.Rate), 0) ,
		ISNULL(MAX(tr.Rate), 0),
		ISNULL(MIN(tr.Rate), 0)
	FROM Currency c
	JOIN [TradeHistory] tr ON tr.CurrencyId = c.Id
	JOIN [TradePair] tp ON tp.Id = tr.TradePairId
	JOIN [TradeHistory] tr2 ON tr2.Id = tr.Id
	WHERE tp.CurrencyId2 = @BaseCurrencyId
	AND tr.Timestamp >= DATEADD(day, -1, GETDATE()) 
	AND c.Id in(SELECT DISTINCT CurrencyId1 FROM TradePair WHERE CurrencyId2 = @BaseCurrencyId)
	GROUP BY c.Id

	-- Select out the results
	SELECT 
	    t.Cid AS CurrencyId,
	    t.Tid AS TradePairId,
		t.Name,
		t.Symbol,
		ISNULL(s.Volume,0) AS Volume,
		ISNULL(s.BaseVolume,0) AS BaseVolume,
		ISNULL(s.High,0) AS High,
		ISNULL(s.Low,0) AS Low
	FROM #temp t
	LEFT JOIN #stats s ON s.Cid = t.Cid


    DROP TABLE #temp
	DROP TABLE #stats

RETURN

GO