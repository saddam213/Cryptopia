CREATE PROCEDURE [dbo].[GetRewards]
AS  
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
    SET NOCOUNT ON;

	SELECT 
		c.Symbol,
		u.UserName,
		r.RewardType,
		r.[Percent],
		r.Amount,
		r.TimeStamp
	FROM Reward r
	JOIN Currency c ON c.Id = r.CurrencyId
	JOIN [User] u ON u.Id = r.UserId
	WHERE u.DisableRewards = 0 AND r.TimeStamp >= DATEADD(DAY, -1, GETDATE())
	ORDER BY r.Id DESC

RETURN
GO