CREATE PROCEDURE [dbo].[AdminGetWithdrawals]
	  @Page			INT = 1 --default to page 1
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
		,UserName
		,Amount
		,Confirmations 
		,Symbol
		,TxId 
		,[TimeStamp]
		,TransactionType
		,[Status] 
		,TotalCount -- Make sure this property is always last
   FROM
   (
	    SELECT 
		     COUNT(*) OVER () AS TotalCount
			,ROW_NUMBER() OVER (
			 ORDER BY 
				  CASE WHEN @ColumnOrderNumber = 0 AND @ColumnOrderAcending = 1 THEN Id END ASC,
				  CASE WHEN @ColumnOrderNumber = 0 AND @ColumnOrderAcending = 0 THEN Id END DESC,
				  CASE WHEN @ColumnOrderNumber = 1 AND @ColumnOrderAcending = 1 THEN [TimeStamp] END ASC,
				  CASE WHEN @ColumnOrderNumber = 1 AND @ColumnOrderAcending = 0 THEN [TimeStamp] END DESC,
				  CASE WHEN @ColumnOrderNumber = 2 AND @ColumnOrderAcending = 1 THEN UserName END ASC,
				  CASE WHEN @ColumnOrderNumber = 2 AND @ColumnOrderAcending = 0 THEN UserName END DESC,
				  CASE WHEN @ColumnOrderNumber = 3 AND @ColumnOrderAcending = 1 THEN Symbol END ASC,
				  CASE WHEN @ColumnOrderNumber = 3 AND @ColumnOrderAcending = 0 THEN Symbol END DESC,
				  CASE WHEN @ColumnOrderNumber = 4 AND @ColumnOrderAcending = 1 THEN Amount END ASC,
				  CASE WHEN @ColumnOrderNumber = 4 AND @ColumnOrderAcending = 0 THEN Amount END DESC,
				  CASE WHEN @ColumnOrderNumber = 5 AND @ColumnOrderAcending = 1 THEN TransactionType END ASC,
				  CASE WHEN @ColumnOrderNumber = 5 AND @ColumnOrderAcending = 0 THEN TransactionType END DESC,
				  CASE WHEN @ColumnOrderNumber = 6 AND @ColumnOrderAcending = 1 THEN TxId END ASC,
				  CASE WHEN @ColumnOrderNumber = 6 AND @ColumnOrderAcending = 0 THEN TxId END DESC,
				  CASE WHEN @ColumnOrderNumber = 7 AND @ColumnOrderAcending = 1 THEN Confirmations END ASC,
				  CASE WHEN @ColumnOrderNumber = 7 AND @ColumnOrderAcending = 0 THEN Confirmations END DESC,
				  CASE WHEN @ColumnOrderNumber = 8 AND @ColumnOrderAcending = 1 THEN [Status] END ASC,
				  CASE WHEN @ColumnOrderNumber = 8 AND @ColumnOrderAcending = 0 THEN [Status] END DESC
			 ) AS RowNum
			,SelectResult.*
		FROM
		(
		    SELECT 
		         w.Id
				,u.UserName
				,w.Amount AS Amount
				,w.Confirmations AS Confirmations
				,c.Symbol AS Symbol 
				,w.TxId as TxId
				,w.[TimeStamp] AS [TimeStamp]
				,wt.Name AS TransactionType
				,ws.Name AS [Status] 
			FROM Withdraw w
			JOIN [User] u on u.Id = w.UserId
			JOIN WithdrawType wt ON wt.Id = w.WithdrawTypeId
			JOIN WithdrawStatus ws ON ws.Id = w.WithdrawStatusId
			JOIN Currency c ON c.Id = w.CurrencyId
			WHERE ((ISNULL(@SearchTerm, '') = '' AND 1=1)
			OR (ISNULL(@SearchTerm, '') <> '' AND 
			c.Name LIKE @Search
			OR u.UserName LIKE @Search
			OR c.Symbol LIKE @Search
			OR w.TxId LIKE @Search
			OR w.Confirmations LIKE @Search
			OR w.Amount LIKE @Search
			OR [Status] LIKE @Search
			OR wt.Name LIKE @Search))
	   ) SelectResult
	) 
	AS RowConstrainedResult
    WHERE RowNum BETWEEN @FirstRow AND @LastRow
    
RETURN

GO



