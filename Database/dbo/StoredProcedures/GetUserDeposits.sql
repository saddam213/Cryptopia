﻿CREATE PROCEDURE [dbo].[GetUserDeposits]
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
         Id   
        ,Amount 
		,Confirmations 
		,CurrencyName 
		,CurrencySymbol 
		,NetworkTransactionID 
		,TransactionType
		,MinConfirmations
		,[Timestamp]
		,[Status]  
		,TotalCount -- Make sure this property is always last
   FROM
   (
	    SELECT 
		     COUNT(*) OVER () AS TotalCount
			,ROW_NUMBER() OVER (
			 ORDER BY 
				  CASE WHEN @ColumnOrderNumber = 0 AND @ColumnOrderAcending = 1 THEN Timestamp END ASC,
				  CASE WHEN @ColumnOrderNumber = 0 AND @ColumnOrderAcending = 0 THEN Timestamp END DESC,
				  CASE WHEN @ColumnOrderNumber = 1 AND @ColumnOrderAcending = 1 THEN CurrencySymbol END ASC,
				  CASE WHEN @ColumnOrderNumber = 1 AND @ColumnOrderAcending = 0 THEN CurrencySymbol END DESC,
				  CASE WHEN @ColumnOrderNumber = 2 AND @ColumnOrderAcending = 1 THEN TransactionType END ASC,
				  CASE WHEN @ColumnOrderNumber = 2 AND @ColumnOrderAcending = 0 THEN TransactionType END DESC,
				  CASE WHEN @ColumnOrderNumber = 3 AND @ColumnOrderAcending = 1 THEN Amount END ASC,
				  CASE WHEN @ColumnOrderNumber = 3 AND @ColumnOrderAcending = 0 THEN Amount END DESC,
				  CASE WHEN @ColumnOrderNumber = 4 AND @ColumnOrderAcending = 1 THEN NetworkTransactionID END ASC,
				  CASE WHEN @ColumnOrderNumber = 4 AND @ColumnOrderAcending = 0 THEN NetworkTransactionID END DESC,
				  CASE WHEN @ColumnOrderNumber = 5 AND @ColumnOrderAcending = 1 THEN Confirmations END ASC,
				  CASE WHEN @ColumnOrderNumber = 5 AND @ColumnOrderAcending = 0 THEN Confirmations END DESC,
				  CASE WHEN @ColumnOrderNumber = 6 AND @ColumnOrderAcending = 1 THEN [Status] END ASC,
				  CASE WHEN @ColumnOrderNumber = 6 AND @ColumnOrderAcending = 0 THEN [Status] END DESC
			 ) AS RowNum
			,SelectResult.*
		FROM
		(
		    SELECT 
				 d.Id
				,d.Amount AS Amount
				,d.Confirmations AS Confirmations
				,c.Name AS CurrencyName
				,c.Symbol AS CurrencySymbol 
				,d.TxId as NetworkTransactionID
				,d.TimeStamp AS Timestamp
				,dt.Name AS TransactionType
				,c.MinConfirmations
				,CASE WHEN  d.Confirmations >= c.MinConfirmations 
					  THEN 'Confirmed'		
					  ELSE 'Unconfirmed'
				 END AS Status 
			FROM Deposit d
			JOIN DepositType dt ON dt.Id = d.DepositTypeId
			JOIN Currency c ON c.Id = d.CurrencyId
			WHERE d.UserId = @UserId
			AND ((ISNULL(@SearchTerm, '') = '' AND 1=1)
			OR (ISNULL(@SearchTerm, '') <> '' AND 
			c.Name LIKE @Search
			OR c.Symbol LIKE @Search
			OR d.TxId LIKE @Search
			OR Confirmations LIKE @Search
			OR Amount LIKE @Search
			OR [Status] LIKE @Search))
	   ) SelectResult
	) 
	AS RowConstrainedResult
    WHERE RowNum BETWEEN @FirstRow AND @LastRow
    
RETURN



GO



