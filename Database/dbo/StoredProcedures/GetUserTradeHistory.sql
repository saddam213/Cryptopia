CREATE PROCEDURE [dbo].[GetUserTradeHistory]
	  @UserId uniqueidentifier
	 ,@Page			INT = 1 --default to page 1
	 ,@ItemsPerPage	INT = 200 -- default 200
	 ,@SearchTerm	NVARCHAR(128) = NULL -- default NULL
	 ,@ColumnOrderNumber INT = 0 -- default 0
	 ,@ColumnOrderAcending	BIT = 0 -- default 0
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

   DECLARE @Search NVARCHAR(130) = '%' + @SearchTerm + '%'
   DECLARE @FirstRow INT = (@ItemsPerPage * (@Page -1)) + 1
   DECLARE @LastRow INT = @ItemsPerPage * @Page

   SELECT   
         TradeId 
		,Symbol1 
		,Symbol2 
		,Rate 
		,Amount 
		,Total
		,TradeType  
		,Timestamp
		,Remaining
		,TradePairId 
		,Fee
		,TotalCount -- Make sure this property is always last
   FROM
   (
	    select 
		     COUNT(*) OVER () AS TotalCount
			,ROW_NUMBER() OVER (
			 ORDER BY 
				  CASE WHEN @ColumnOrderNumber = 0 AND @ColumnOrderAcending = 1 THEN Timestamp END ASC,
				  CASE WHEN @ColumnOrderNumber = 0 AND @ColumnOrderAcending = 0 THEN Timestamp END DESC,
				  CASE WHEN @ColumnOrderNumber = 1 AND @ColumnOrderAcending = 1 THEN Symbol1 + '/' + Symbol2 END ASC,
				  CASE WHEN @ColumnOrderNumber = 1 AND @ColumnOrderAcending = 0 THEN Symbol1 + '/' + Symbol2 END DESC,
				  CASE WHEN @ColumnOrderNumber = 2 AND @ColumnOrderAcending = 1 THEN TradeType END ASC,
				  CASE WHEN @ColumnOrderNumber = 2 AND @ColumnOrderAcending = 0 THEN TradeType END DESC,
				  CASE WHEN @ColumnOrderNumber = 3 AND @ColumnOrderAcending = 1 THEN Rate END ASC,
				  CASE WHEN @ColumnOrderNumber = 3 AND @ColumnOrderAcending = 0 THEN Rate END DESC,
				  CASE WHEN @ColumnOrderNumber = 4 AND @ColumnOrderAcending = 1 THEN Amount END ASC,
				  CASE WHEN @ColumnOrderNumber = 4 AND @ColumnOrderAcending = 0 THEN Amount END DESC,
				  CASE WHEN @ColumnOrderNumber = 5 AND @ColumnOrderAcending = 1 THEN Total END ASC,
				  CASE WHEN @ColumnOrderNumber = 5 AND @ColumnOrderAcending = 0 THEN Total END DESC
			 ) AS RowNum
			,SelectResult.*
		FROM
		(
			SELECT 
			     o.Timestamp AS Timestamp 
				 , c1.Symbol AS Symbol1 
				 , c2.Symbol AS Symbol2 
				 , o.Rate AS Rate 
				 , o.Amount AS Amount 
				 , o.Amount * o.Rate AS Total 
				 , CONVERT(TINYINT,0) AS TradeType 
				 , 0 as Remaining
				 , o.Id as TradeId 
				 , t.Id AS TradePairId 
				 , o.Fee
			FROM [dbo].TradeHistory o
			JOIN TradeHistoryType tt ON o.TradeHistoryTypeId = tt.Id
			JOIN [dbo].[TradePair] t ON o.TradePairId = t.Id
			JOIN [dbo].[Currency] c1 ON t.CurrencyId1 = c1.Id
			JOIN [dbo].[Currency] c2 ON t.CurrencyId2 = c2.Id
			WHERE o.UserId = @UserId
			AND ((ISNULL(@SearchTerm, '') = '' AND 1=1)
			OR (ISNULL(@SearchTerm, '') <> '' AND 
			c1.Symbol + '/' + c2.Symbol LIKE @Search
			OR o.Rate LIKE @Search
			OR o.Amount LIKE @Search
			OR tt.Name LIKE @Search
			OR (o.Amount * o.Rate) LIKE @Search))

			UNION ALL

			SELECT 
			     o.Timestamp AS Timestamp 
				 , c1.Symbol AS Symbol1 
				 , c2.Symbol AS Symbol2 
				 , o.Rate AS Rate 
				 , o.Amount AS Amount 
				 , o.Amount * o.Rate AS Total 
			     , CONVERT(TINYINT,1) AS TradeType 
				 , 0 as Remaining
				 , o.Id as TradeId 
				 , t.Id AS TradePairId 
				 , o.Fee
			FROM [dbo].TradeHistory o
			JOIN TradeHistoryType tt ON o.TradeHistoryTypeId = tt.Id
			JOIN [dbo].[TradePair] t ON o.TradePairId = t.Id
			JOIN [dbo].[Currency] c1 ON t.CurrencyId1 = c1.Id
			JOIN [dbo].[Currency] c2 ON t.CurrencyId2 = c2.Id
			WHERE o.ToUserId = @UserId
			AND ((ISNULL(@SearchTerm, '') = '' AND 1=1)
			OR (ISNULL(@SearchTerm, '') <> '' AND 
			c1.Symbol + '/' + c2.Symbol LIKE @Search
			OR o.Rate LIKE @Search
			OR o.Amount LIKE @Search
			OR tt.Name LIKE @Search
			OR (o.Amount * o.Rate) LIKE @Search))
	  ) SelectResult
	) 
	AS RowConstrainedResult
    WHERE RowNum BETWEEN @FirstRow AND @LastRow
	
RETURN

GO


