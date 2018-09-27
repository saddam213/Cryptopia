CREATE PROCEDURE [dbo].[GetMarketCategories]
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

   SELECT  
         mc.Id 
		,mc.ParentId 
		,mc.Name 
		,mc.DisplayName 
		,(SELECT COUNT(1)  
			FROM MarketItem m
			JOIN MarketCategory mc1 on mc1.Id = m.CategoryId
			JOIN MarketCategory mc2 on mc2.Id = mc1.ParentId
			JOIN MarketCategory mc3 on mc3.Id = mc2.ParentId
		 WHERE m.MarketItemStatusId = 0 --Active
		 AND mc.Id in ( mc1.Id, mc2.Id, mc3.Id))
		 AS ItemCount
   FROM MarketCategory mc
   ORDER BY mc.Id
   
RETURN
GO



