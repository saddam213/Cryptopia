CREATE PROCEDURE [dbo].[GetUserMarketFeedback]
      @UserName NVARCHAR(256)
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
         MarketItemId
        ,Rating 
		,Comment 
		,Sender 
		,Receiver 
		,SenderTrustRating
		,Timestamp 
		,TotalCount -- Make sure this property is always last
   FROM
   (
	    SELECT 
		     COUNT(*) OVER () AS TotalCount
			,ROW_NUMBER() OVER (ORDER BY Timestamp DESC) AS RowNum
			,SelectResult.*
		FROM
		(
			SELECT 
				 mf.MarketItemId AS MarketItemId
				,mf.Rating AS Rating
				,mf.Comment AS Comment
				,us.UserName AS Sender 
				,ur.UserName AS Receiver
				,us.TrustRating AS SenderTrustRating
				,mf.Timestamp AS Timestamp
			FROM MarketFeedback mf
			JOIN [User] us on us.Id = mf.SenderUserId
			JOIN [User] ur on ur.Id = mf.ReceiverUserId
			WHERE ur.UserName = @UserName
	   ) SelectResult
	) 
	AS RowConstrainedResult
    WHERE RowNum BETWEEN @FirstRow AND @LastRow
    
RETURN

GO



