CREATE PROCEDURE [dbo].[GetRewardStatistics]
@Days INT = 1
AS  
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
    SET NOCOUNT ON;

	SELECT 
		 u.UserName  
		,RewardType
		,COUNT(1) AS [Count]
	FROM Reward r
	JOIN [User] u ON u.Id = r.UserId
	WHERE u.DisableRewards = 0 AND r.TimeStamp >= DATEADD(DAY, -@Days, GETDATE())
	GROUP BY RewardType, u.UserName 
	ORDER BY 1 DESC

RETURN
GO

