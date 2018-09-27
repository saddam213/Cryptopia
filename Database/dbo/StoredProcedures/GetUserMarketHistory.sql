CREATE PROCEDURE [dbo].[GetUserMarketHistory]
      @UserId UNIQUEIDENTIFIER = NULL	
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
       	 Id
		,Title
		,CloseDate
		,Created 
		,[Type]
		,[Status]
		,Buyer
		,BuyersRating
		,Seller
		,SellersRating
		,TotalCount -- Make sure this property is always last
   FROM
   (
	    SELECT 
		     COUNT(*) OVER () AS TotalCount
			,ROW_NUMBER() OVER (
			 ORDER BY 
			      CASE WHEN @ColumnOrderNumber = 0 AND @ColumnOrderAcending = 1 THEN Id END ASC,
				  CASE WHEN @ColumnOrderNumber = 0 AND @ColumnOrderAcending = 0 THEN Id END DESC,
				  CASE WHEN @ColumnOrderNumber = 1 AND @ColumnOrderAcending = 1 THEN Title END ASC,
				  CASE WHEN @ColumnOrderNumber = 1 AND @ColumnOrderAcending = 0 THEN Title END DESC,
				  CASE WHEN @ColumnOrderNumber = 2 AND @ColumnOrderAcending = 1 THEN [Type] END ASC,
				  CASE WHEN @ColumnOrderNumber = 2 AND @ColumnOrderAcending = 0 THEN [Type] END DESC,
				  CASE WHEN @ColumnOrderNumber = 3 AND @ColumnOrderAcending = 1 THEN Buyer END ASC,
				  CASE WHEN @ColumnOrderNumber = 3 AND @ColumnOrderAcending = 0 THEN Buyer END DESC,
				  CASE WHEN @ColumnOrderNumber = 4 AND @ColumnOrderAcending = 1 THEN Seller END ASC,
				  CASE WHEN @ColumnOrderNumber = 4 AND @ColumnOrderAcending = 0 THEN Seller END DESC,
				  CASE WHEN @ColumnOrderNumber = 7 AND @ColumnOrderAcending = 1 THEN CloseDate END ASC,
				  CASE WHEN @ColumnOrderNumber = 7 AND @ColumnOrderAcending = 0 THEN CloseDate END DESC,
				  CASE WHEN @ColumnOrderNumber = 8 AND @ColumnOrderAcending = 1 THEN [Status] END ASC,
				  CASE WHEN @ColumnOrderNumber = 8 AND @ColumnOrderAcending = 0 THEN [Status] END DESC
			 ) AS RowNum
			,SelectResult.*
		FROM
		(
			SELECT 
				 m.Id
				,m.Title
				,mb.Timestamp as CloseDate
				,m.Created 
				,mt.Name as [Type]
				,ms.Name as [Status]
				,ur.UserName as Buyer
				,mfr.Rating as BuyersRating
				,us.UserName as Seller
				,mfs.Rating as SellersRating
			FROM MarketItem m
			JOIN MarketItemType mt ON mt.Id = m.MarketItemTypeId
			JOIN MarketItemStatus ms ON ms.Id = m.MarketItemStatusId
			JOIN MarketItemBid mb on mb.MarketItemId = m.Id AND mb.IsWinningBid = 1
			JOIN [User] us on us.Id = m.UserId
			JOIN [User] ur on ur.Id = mb.UserId
			LEFT JOIN MarketFeedback mfs on mfs.MarketItemId = m.Id AND mfs.ReceiverUserId = us.Id
			LEFT JOIN MarketFeedback mfr on mfr.MarketItemId = m.Id AND mfr.ReceiverUserId = ur.Id
			where ms.Id = 10 -- complete 
			AND m.UserId = @UserId OR mb.UserId = @UserId
			AND ((ISNULL(@SearchTerm, '') = '' AND 1=1)
			OR (ISNULL(@SearchTerm, '') <> '' AND 
				m.Title LIKE @Search
				OR ur.Username LIKE @Search
				OR us.Username LIKE @Search))
			
	   ) SelectResult
	) 
	AS RowConstrainedResult
    WHERE RowNum BETWEEN @FirstRow AND @LastRow
    
RETURN
GO



