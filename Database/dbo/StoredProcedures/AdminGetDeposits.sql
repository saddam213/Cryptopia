CREATE PROCEDURE [dbo].[AdminGetDeposits]
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
				  CASE WHEN @ColumnOrderNumber = 6 AND @ColumnOrderAcending = 1 THEN Confirmations END ASC,
				  CASE WHEN @ColumnOrderNumber = 6 AND @ColumnOrderAcending = 0 THEN Confirmations END DESC,
				  CASE WHEN @ColumnOrderNumber = 7 AND @ColumnOrderAcending = 1 THEN [Status] END ASC,
				  CASE WHEN @ColumnOrderNumber = 7 AND @ColumnOrderAcending = 0 THEN [Status] END DESC,
				  CASE WHEN @ColumnOrderNumber = 8 AND @ColumnOrderAcending = 1 THEN TxId END ASC,
				  CASE WHEN @ColumnOrderNumber = 8 AND @ColumnOrderAcending = 0 THEN TxId END DESC
			 ) AS RowNum
			 
			,SelectResult.*
		FROM
		(
		    SELECT
		         d.Id
				,u.UserName
				,d.Amount AS Amount
				,d.Confirmations AS Confirmations
				,c.Symbol AS Symbol 
				,d.TxId as TxId
				,d.[TimeStamp] AS [TimeStamp]
				,dt.Name AS TransactionType
				,CASE WHEN  d.Confirmations >= c.MinConfirmations 
					  THEN 'Confirmed'		
					  ELSE 'Unconfirmed'
				 END AS [Status] 
			FROM Deposit d
			JOIN [User] u on u.Id = d.UserId
			JOIN DepositType dt ON dt.Id = d.DepositTypeId
			JOIN Currency c ON c.Id = d.CurrencyId
			WHERE ((ISNULL(@SearchTerm, '') = '' AND 1=1)
			OR (ISNULL(@SearchTerm, '') <> '' AND 
			c.Name LIKE @Search
			OR u.UserName LIKE @Search
			OR c.Symbol LIKE @Search
			OR d.TxId LIKE @Search
			OR d.Confirmations LIKE @Search
			OR d.Amount LIKE @Search
			OR [Status] LIKE @Search
			OR dt.Name LIKE @Search))
	   ) SelectResult
	) 
	AS RowConstrainedResult
    WHERE RowNum BETWEEN @FirstRow AND @LastRow
    
RETURN