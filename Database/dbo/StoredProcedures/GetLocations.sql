CREATE PROCEDURE [dbo].[GetLocations]
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

   SELECT   
         Id 
		,ParentId 
		,Name 
		,CountryCode 
   FROM Location l
 
   
RETURN
GO



