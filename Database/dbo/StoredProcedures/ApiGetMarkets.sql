CREATE PROCEDURE [dbo].[ApiGetMarkets]
	@Hours INT = 24,
	@TradePairId INT = NULL
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	 CREATE TABLE #temp
	(
	  Id INT,
	  Label NVARCHAR(50),
	  AskPrice DECIMAL(38,8) DEFAULT(0),
	  BidPrice DECIMAL(38,8) DEFAULT(0),
	  Low DECIMAL(38,8) DEFAULT(0),
	  High DECIMAL(38,8) DEFAULT(0),
	  Volume DECIMAL(38,8) DEFAULT(0),
	  LastPrice DECIMAL(38,8) DEFAULT(0),
	  LastVolume DECIMAL(38,8) DEFAULT(0),
	  BuyVolume DECIMAL(38,8) DEFAULT(0),
	  SellVolume DECIMAL(38,8) DEFAULT(0),
	  Change DECIMAL(38,8) DEFAULT(0)
	)
   
   -- Get TradePair details
   INSERT INTO #temp (Id, Label, LastPrice, Change, LastVolume)
   SELECT 
		tp.Id,
		c1.Symbol + '/' + c2.Symbol,
		tp.LastTrade,
		tp.Change,
		ISNULL((SELECT TOP 1 Amount FROM TradeHistory WHERE TradePairId = tp.Id ORDER BY ID DESC), 0)
   FROM TradePair tp
   JOIN Currency c1 ON c1.Id = tp.CurrencyId1 
   JOIN Currency c2 ON c2.Id = tp.CurrencyId2 
   WHERE @TradePairId IS NULL OR @TradePairId = tp.Id
 
    -- Get BidPrice and BuyVolume
	MERGE INTO #temp tb
		USING (SELECT 
			t.TradePairId,
			MAX(t.Rate) AS Bid,
			SUM(Amount) AS Vol
			FROM Trade t
			WHERE @TradePairId IS NULL OR @TradePairId = t.TradePairId
			AND t.TradeTypeId = 0 AND T.TradeStatusId IN (0,2)
			GROUP BY t.TradePairId) buy
	ON tb.Id = buy.TradePairId
	WHEN MATCHED THEN
	UPDATE 
	SET BidPrice = buy.Bid, 
		BuyVolume = buy.vol;

	-- Get AskPrice and SellVolume
	MERGE INTO #temp tb
		USING (SELECT 
			t.TradePairId,
			MIN(t.Rate) AS Bid,
			SUM(Amount) AS Vol
			FROM Trade t
			WHERE @TradePairId IS NULL OR @TradePairId = t.TradePairId
			AND t.TradeTypeId = 1 AND T.TradeStatusId IN (0,2)
			GROUP BY t.TradePairId) sell
	ON tb.Id = sell.TradePairId
	WHEN MATCHED THEN
	UPDATE 
	SET AskPrice = sell.Bid, 
		SellVolume = sell.vol;

	-- Get High, Low and Volume
	MERGE INTO #temp tb
		USING (SELECT
			c.Id,
			ISNULL(SUM(tr.Amount ),0) + ISNULL(SUM(tr.Amount  * tr.Rate), 0) as Volume,
			ISNULL(MAX(tr.Rate), 0) AS High,
			ISNULL(MIN(tr.Rate), 0) AS Low
		FROM TradePair c
		JOIN [TradeHistory] tr ON tr.TradePairId = c.Id
		WHERE @TradePairId IS NULL OR @TradePairId = c.Id
		AND tr.Timestamp >= DATEADD(hour, -@Hours, GETDATE()) 
		GROUP BY c.Id) Vol
	ON tb.Id = Vol.Id
	WHEN MATCHED THEN
	UPDATE 
	SET Low = Vol.Low, 
		High = Vol.High,
		Volume = Vol.Volume;  
   
   -- Select Results
   SELECT
      Id as TradePairId,
	  Label,
	  AskPrice,
	  BidPrice,
	  Low,
	  High,
	  Volume,
	  LastPrice,
	  LastVolume,
	  BuyVolume,
	  SellVolume,
	  Change
   FROM #temp

   DROP TABLE #temp
    
RETURN

GO



