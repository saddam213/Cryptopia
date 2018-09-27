CREATE PROCEDURE [dbo].[GetMarketItems]
   @CategoryId INT = 0
  ,@CurrencyId INT = 0
  ,@LocationId INT = 0
  ,@MarketItemTypeId INT = NULL
  ,@SortBy NVARCHAR(128) = NULL
  ,@Page   INT = 1 --default to page 1
  ,@ItemsPerPage INT = 200 -- default 200
  ,@SearchTerm NVARCHAR(128) = NULL -- default NULL
  ,@ColumnOrderNumber INT = 0 -- default 0
  ,@ColumnOrderAcending BIT = 0 -- default 0
AS  
    SET NOCOUNT ON;
 SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	DECLARE @AdultCategoryId INT = 99
	DECLARE @Search NVARCHAR(130) = '%' + @SearchTerm + '%'
    DECLARE @FirstRow INT = (@ItemsPerPage * (@Page -1)) + 1
    DECLARE @LastRow INT = @ItemsPerPage * @Page

   SELECT   
         Id 
	  ,CategoryId 
	  ,LocationId 
	  ,Title 
	  ,[Description]
	  ,MainImage
	  ,AskingPrice 
	  ,ReservePrice
	  ,CurrentBidPrice
	  ,[Type]
	  ,[Status]
	  ,Feature
	  ,CloseDate
	  ,Created
	  ,Symbol
	  ,TotalCount -- Make sure this property is always last
   FROM
   (
		 SELECT 
		   COUNT(*) OVER () AS TotalCount
		   ,ROW_NUMBER() OVER (
			ORDER BY Feature DESC,
			  CASE WHEN @SortBy IS NULL OR @SortBy = 'Title' THEN Title END ASC,
			  CASE WHEN @SortBy = 'Highest Price' THEN AskingPrice END DESC,
			  CASE WHEN @SortBy = 'Lowest Price'  THEN AskingPrice END ASC,
			  CASE WHEN @SortBy = 'Closing Soon'  THEN CloseDate END ASC,
			  CASE WHEN @SortBy = 'Newest'   THEN Created END DESC
			, Title ASC
			) AS RowNum
	   ,SelectResult.*
  FROM
  (
	 SELECT 
		 m.Id
		,m.CategoryId
		,m.LocationId
		,m.Title
		,m.Description
		,m.MainImage
		,m.AskingPrice
		,m.ReservePrice 
		, (SELECT MAX(BidAmount) FROM MarketItemBid WHERE MarketItemId = m.Id) AS CurrentBidPrice
		,m.CloseDate
		,m.Created 
		,mf.Id as Feature
		,mt.Id as [Type]
		,m.MarketItemStatusId as [Status]
		,c.Id as CurrencyId
		,c.Symbol as Symbol
	   FROM MarketItem m
	   JOIN MarketItemType mt ON mt.Id = m.MarketItemTypeId
	   JOIN MarketItemStatus ms ON ms.Id = m.MarketItemStatusId
	   JOIN MarketItemFeature mf ON mf.Id = m.MarketItemFeatureId
	   JOIN Currency c on c.Id = m.CurrencyId
	   JOIN MarketCategory mc1 on mc1.Id = m.CategoryId
	   JOIN MarketCategory mc2 on mc2.Id = mc1.ParentId
	   JOIN MarketCategory mc3 on mc3.Id = mc2.ParentId
	   JOIN Location l1 on l1.Id = m.LocationId
	   LEFT JOIN Location l2 on l2.Id = l1.ParentId
	   WHERE m.MarketItemStatusId = 0 -- Active 
	   AND (@MarketItemTypeId IS NULL OR @MarketItemTypeId = m.MarketItemTypeId)
	   AND ((@CurrencyId > 0 AND @CurrencyId = m.CurrencyId) OR (ISNULL(@CurrencyId,0) = 0))
	   AND ((@CategoryId > 0 AND @CategoryId in ( mc1.Id, mc2.Id, mc3.Id)) OR (ISNULL(@CategoryId,0) = 0 and @AdultCategoryId not in ( mc1.Id, mc2.Id, mc3.Id)))
	   AND ((@LocationId > 0 AND @LocationId in ( l1.Id, l2.Id)) OR (ISNULL(@LocationId,0) = 0))
	   AND ((ISNULL(@SearchTerm, '') = '' AND 1=1)
	   OR (ISNULL(@SearchTerm, '') <> '' AND 
	   m.Title LIKE @Search
	   OR m.Description LIKE @Search))
    	) SelectResult
 ) 
 AS RowConstrainedResult
    WHERE RowNum BETWEEN @FirstRow AND @LastRow
    
RETURN
GO



