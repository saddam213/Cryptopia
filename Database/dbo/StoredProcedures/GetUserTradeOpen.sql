CREATE PROCEDURE [dbo].[GetUserTradeOpen]
	  @UserId UNIQUEIDENTIFIER
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
		,TotalCount -- Make sure this property is always last
   FROM
   (
	    SELECT 
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
			   o.Id AS TradeId 
			 , c1.Symbol AS Symbol1 
			 , c2.Symbol AS Symbol2 
			 , o.Rate AS Rate 
			 , o.Amount AS Amount 
			 , o.Amount * o.Rate AS Total 
			 , o.TradeTypeId AS TradeType 
			 , o.Timestamp AS Timestamp 
			 , o.Remaining AS Remaining 
			 , t.Id AS TradePairId 
			FROM [dbo].[Trade] o
			JOIN TradeHistoryType tt ON o.TradeTypeId = tt.Id
			JOIN [dbo].[TradePair] t ON o.TradePairId = t.Id
			JOIN [dbo].[Currency] c1 ON t.CurrencyId1 = c1.Id
			JOIN [dbo].[Currency] c2 ON t.CurrencyId2 = c2.Id
			WHERE o.UserId = @UserId AND o.TradeStatusId in(0,2)
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


