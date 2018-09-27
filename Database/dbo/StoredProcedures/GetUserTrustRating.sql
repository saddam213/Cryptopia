CREATE PROCEDURE [dbo].[GetUserTrustRating]
      @UserName NVARCHAR(256)
AS  
    SET NOCOUNT ON;
 SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

 SELECT
	SUM(Rating) / CONVERT(FLOAT, COUNT(1)) AS TotalMarketRating
 FROM MarketFeedback mf
 JOIN [User] u ON u.Id = mf.ReceiverUserId
 WHERE u.UserName = @UserName
    
RETURN
GO



