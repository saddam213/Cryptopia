CREATE PROCEDURE [dbo].[AdminGetErrorLogs]
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
		,Component
		,Method
		,Request 
		,Exception
		,[TimeStamp]
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
				  CASE WHEN @ColumnOrderNumber = 2 AND @ColumnOrderAcending = 1 THEN Component END ASC,
				  CASE WHEN @ColumnOrderNumber = 2 AND @ColumnOrderAcending = 0 THEN Component END DESC,
				  CASE WHEN @ColumnOrderNumber = 3 AND @ColumnOrderAcending = 1 THEN Method END ASC,
				  CASE WHEN @ColumnOrderNumber = 3 AND @ColumnOrderAcending = 0 THEN Method END DESC,
				  CASE WHEN @ColumnOrderNumber = 4 AND @ColumnOrderAcending = 1 THEN Request END ASC,
				  CASE WHEN @ColumnOrderNumber = 4 AND @ColumnOrderAcending = 0 THEN Request END DESC,
				  CASE WHEN @ColumnOrderNumber = 5 AND @ColumnOrderAcending = 1 THEN Exception END ASC,
				  CASE WHEN @ColumnOrderNumber = 5 AND @ColumnOrderAcending = 0 THEN Exception END DESC
			 ) AS RowNum
			,SelectResult.*
		FROM
		(
		    SELECT 
		       	 e.Id
				,e.Component
				,e.Method
				,e.Request 
				,e.Exception
				,e.[Timestamp]
			FROM ErrorLog e
			WHERE ((ISNULL(@SearchTerm, '') = '' AND 1=1)
			OR (ISNULL(@SearchTerm, '') <> '' AND 
			e.Component LIKE @Search
			OR e.Method LIKE @Search
			OR e.Request LIKE @Search
			OR e.Exception LIKE @Search))
	   ) SelectResult
	) 
	AS RowConstrainedResult
    WHERE RowNum BETWEEN @FirstRow AND @LastRow
    
RETURN

GO



