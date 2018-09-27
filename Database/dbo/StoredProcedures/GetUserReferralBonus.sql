CREATE PROCEDURE [dbo].[GetUserReferralBonus]
 @UserId UNIQUEIDENTIFIER
AS
BEGIN

	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	DECLARE @UserName NVARCHAR(50)

	SELECT @UserName = UserName
		FROM [User]
		WHERE Id = @UserId

	DECLARE @ActiveUsers TABLE (UserName NVARCHAR(Max), TradeCount int)
	DECLARE @ActiveUsersReward int
	DECLARE @ActiveTradesReward int

	INSERT INTO @ActiveUsers (UserName, TradeCount)
	(SELECT U.UserName, COUNT(*)
		FROM [User] U
		JOIN TradeHistory TH1 ON TH1.UserId = U.Id
		JOIN TradeHistory TH2 ON TH2.ToUserId = U.Id
		WHERE (TH1.UserId IN (SELECT Id
							FROM [User]
							WHERE [Referrer] = @UserName)
		OR TH1.ToUserId IN (SELECT Id
							FROM [User]
							WHERE [Referrer] = @UserName))
		AND (TH2.UserId IN (SELECT Id
							FROM [User]
							WHERE [Referrer] = @UserName)
		OR TH2.ToUserId IN (SELECT Id
							FROM [User]
							WHERE [Referrer] = @UserName))
		AND TH1.ToUserId != TH1.UserId
		AND TH2.ToUserId != TH2.UserId
		AND TH1.Timestamp > '2014-12-28 09:00'
		AND TH2.Timestamp > '2014-12-28 09:00'
		GROUP BY U.UserName)

	SELECT @ActiveUsersReward = COUNT(DISTINCT UserName)
									FROM @ActiveUsers


	SELECT @ActiveTradesReward = SUM(TradeCount)
	FROM @ActiveUsers
		
	SELECT CONVERT(DECIMAL(38,8),(ISNULL(@ActiveTradesReward, 0) * 20) + (ISNULL(@ActiveUsersReward, 0) * 100))
END

GO