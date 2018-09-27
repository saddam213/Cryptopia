CREATE PROCEDURE [dbo].[GetUserTransferHistory]
      @UserId UNIQUEIDENTIFIER
     ,@RewardsOnly	BIT = 0
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
          Id,
          CurrencyId ,
          Symbol,
          UserTo ,
          UserFrom, 
          Amount ,
          Fee ,
          TransferType,
          TimeStamp 
		,TotalCount -- Make sure this property is always last
   FROM
   (
	    SELECT 
		     COUNT(*) OVER () AS TotalCount
			,ROW_NUMBER() OVER (
			 ORDER BY 
				  CASE WHEN @ColumnOrderNumber = 0 AND @ColumnOrderAcending = 1 THEN Id END ASC,
				  CASE WHEN @ColumnOrderNumber = 0 AND @ColumnOrderAcending = 0 THEN Id END DESC,
				  CASE WHEN @ColumnOrderNumber = 1 AND @ColumnOrderAcending = 1 THEN Symbol END ASC,
				  CASE WHEN @ColumnOrderNumber = 1 AND @ColumnOrderAcending = 0 THEN Symbol END DESC,
				  CASE WHEN @ColumnOrderNumber = 2 AND @ColumnOrderAcending = 1 THEN UserTo END ASC,
				  CASE WHEN @ColumnOrderNumber = 2 AND @ColumnOrderAcending = 0 THEN UserTo END DESC,
				  CASE WHEN @ColumnOrderNumber = 3 AND @ColumnOrderAcending = 1 THEN Amount END ASC,
				  CASE WHEN @ColumnOrderNumber = 3 AND @ColumnOrderAcending = 0 THEN Amount END DESC,
				  CASE WHEN @ColumnOrderNumber = 4 AND @ColumnOrderAcending = 1 THEN UserFrom END ASC,
				  CASE WHEN @ColumnOrderNumber = 4 AND @ColumnOrderAcending = 0 THEN UserFrom END DESC,
				  CASE WHEN @ColumnOrderNumber = 5 AND @ColumnOrderAcending = 1 THEN Fee END ASC,
				  CASE WHEN @ColumnOrderNumber = 5 AND @ColumnOrderAcending = 0 THEN Fee END DESC,
				  CASE WHEN @ColumnOrderNumber = 6 AND @ColumnOrderAcending = 1 THEN TimeStamp END ASC,
				  CASE WHEN @ColumnOrderNumber = 6 AND @ColumnOrderAcending = 0 THEN TimeStamp END DESC
			 ) AS RowNum
			,SelectResult.*
		FROM
		(
			SELECT 
			     th.Id
				,c.Id AS CurrencyId
				,c.Symbol
				,uto.ChatHandle AS UserTo
				,ufrom.ChatHandle AS UserFrom
				,th.Amount
				,th.Fee AS Fee
				,th.TransferType
				,th.Timestamp
			FROM TransferHistory th
			JOIN TransferHistoryType tht on tht.Id = th.TransferType
			JOIN Currency c ON c.Id = th.CurrencyId
			JOIN [User] ufrom on ufrom.Id = th.UserId
			JOIN [User] uto on uto.Id = th.ToUserId
			WHERE (th.UserId = @UserId OR th.ToUserId = @UserId)
			AND ((@RewardsOnly = 0 AND th.TransferType NOT IN (3,4)) OR (@RewardsOnly = 1 AND th.TransferType IN (3,4)))
			AND ((ISNULL(@SearchTerm, '') = '')	
			OR (ISNULL(@SearchTerm, '') <> '' 
			AND (c.Symbol LIKE @Search OR ufrom.ChatHandle LIKE @Search 
			OR uto.ChatHandle LIKE @Search
			OR th.Amount LIKE @Search
			OR tht.Name LIKE @Search)))
	   ) SelectResult
	) 
	AS RowConstrainedResult
    WHERE RowNum BETWEEN @FirstRow AND @LastRow
    
RETURN

GO