CREATE PROCEDURE [dbo].[GetMarketItemBids]
      @MarketItemId INT
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT 
	    mb.Id,
		u.UserName,
		u.TrustRating,
		mb.BidAmount as Amount,
		mb.IsWinningBid,
		mb.UserId,
		mb.Timestamp
	FROM MarketItemBid mb
	JOIN [User] u on u.Id = mb.UserId
	WHERE mb.MarketItemId = @MarketItemId
	ORDER BY mb.Id DESC
  
RETURN

GO



