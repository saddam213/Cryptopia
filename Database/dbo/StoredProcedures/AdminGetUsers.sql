CREATE PROCEDURE [dbo].[AdminGetUsers]
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
		 UserName,
		 ChatHandle,
		 MiningHandle,
		 Email,
		 ISNULL(Referrer,'None') as Referrer,
		 Theme,
		 SettingsTFA,
		 LoginTFA,
		 LockoutTFA,
		 WithdrawTFA,
		 TotalCount -- Make sure this property is always last
   FROM
   (
	    SELECT 
		     COUNT(*) OVER () AS TotalCount
			,ROW_NUMBER() OVER (
			 ORDER BY 
				
				  CASE WHEN @ColumnOrderNumber = 0 AND @ColumnOrderAcending = 1 THEN UserName END ASC,
				  CASE WHEN @ColumnOrderNumber = 0 AND @ColumnOrderAcending = 0 THEN UserName END DESC,
				  CASE WHEN @ColumnOrderNumber = 1 AND @ColumnOrderAcending = 1 THEN ChatHandle END ASC,
				  CASE WHEN @ColumnOrderNumber = 1 AND @ColumnOrderAcending = 0 THEN ChatHandle END DESC,
				  CASE WHEN @ColumnOrderNumber = 2 AND @ColumnOrderAcending = 1 THEN MiningHandle END ASC,
				  CASE WHEN @ColumnOrderNumber = 2 AND @ColumnOrderAcending = 0 THEN MiningHandle END DESC,
				  CASE WHEN @ColumnOrderNumber = 3 AND @ColumnOrderAcending = 1 THEN Email END ASC,
				  CASE WHEN @ColumnOrderNumber = 3 AND @ColumnOrderAcending = 0 THEN Email END DESC,
				  CASE WHEN @ColumnOrderNumber = 4 AND @ColumnOrderAcending = 1 THEN Referrer END ASC,
				  CASE WHEN @ColumnOrderNumber = 4 AND @ColumnOrderAcending = 0 THEN Referrer END DESC,
				  CASE WHEN @ColumnOrderNumber = 5 AND @ColumnOrderAcending = 1 THEN Theme END ASC,
				  CASE WHEN @ColumnOrderNumber = 5 AND @ColumnOrderAcending = 0 THEN Theme END DESC,
				  CASE WHEN @ColumnOrderNumber = 6 AND @ColumnOrderAcending = 1 THEN SettingsTFA END ASC,
				  CASE WHEN @ColumnOrderNumber = 6 AND @ColumnOrderAcending = 0 THEN SettingsTFA END DESC,
				  CASE WHEN @ColumnOrderNumber = 7 AND @ColumnOrderAcending = 1 THEN LoginTFA END ASC,
				  CASE WHEN @ColumnOrderNumber = 7 AND @ColumnOrderAcending = 0 THEN LoginTFA END DESC,
				  CASE WHEN @ColumnOrderNumber = 8 AND @ColumnOrderAcending = 1 THEN LockoutTFA END ASC,
				  CASE WHEN @ColumnOrderNumber = 8 AND @ColumnOrderAcending = 0 THEN LockoutTFA END DESC,
				  CASE WHEN @ColumnOrderNumber = 9 AND @ColumnOrderAcending = 1 THEN WithdrawTFA END ASC,
				  CASE WHEN @ColumnOrderNumber = 9 AND @ColumnOrderAcending = 0 THEN WithdrawTFA END DESC
			 ) AS RowNum
			,SelectResult.*
		FROM
		(
           SELECT 
				u.UserName,
				u.ChatHandle,
				u.MiningHandle,
				u.Email,
				u.Referrer,
				us.Theme,
				ut1.[Type] As SettingsTFA,
				ut2.[Type] As LoginTFA,
				ut3.[Type] As LockoutTFA,
				ut4.[Type] As WithdrawTFA
			FROM [CryptopiaHub].[dbo].AspNetUsers u
			LEFT JOIN [CryptopiaHub].[dbo].UserSettings us on us.Id = u.Id
			LEFT JOIN [CryptopiaHub].[dbo].UserTwoFactor ut1 on ut1.UserId = u.Id and ut1.Component = 0 --Settings
			LEFT JOIN [CryptopiaHub].[dbo].UserTwoFactor ut2 on ut2.UserId = u.Id and ut2.Component = 1 --Login
			LEFT JOIN [CryptopiaHub].[dbo].UserTwoFactor ut3 on ut3.UserId = u.Id and ut3.Component = 5 --Lockout
			LEFT JOIN [CryptopiaHub].[dbo].UserTwoFactor ut4 on ut4.UserId = u.Id and ut4.Component = 20 --Withdraw
			WHERE ((ISNULL(@SearchTerm, '') = '' AND 1=1)
			OR (ISNULL(@SearchTerm, '') <> '' 
			AND u.Id LIKE @Search
			OR u.UserName LIKE @Search
			OR u.ChatHandle LIKE @Search
			OR u.MiningHandle LIKE @Search
			OR u.Email LIKE @Search
			OR u.Referrer LIKE @Search
			OR us.Theme LIKE @Search
			OR ut1.Component LIKE @Search
			OR ut2.Component LIKE @Search
			OR ut3.Component LIKE @Search
			OR ut4.Component LIKE @Search))
	   ) SelectResult
	) 
	AS RowConstrainedResult
    WHERE RowNum BETWEEN @FirstRow AND @LastRow
    
RETURN


GO



