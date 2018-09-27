CREATE PROCEDURE [dbo].[GetUserBalances]
	@UserId UNIQUEIDENTIFIER
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    SELECT 
		 c.Id AS CurrencyId
		,c.Symbol AS Symbol
		,c.Name AS Name 
		,b.Total AS Total
		,b.Total - (b.Unconfirmed + b.HeldForTrades + b.PendingWithdraw) AS Available
		,b.HeldForTrades AS HeldForTrades
		,b.Unconfirmed AS Unconfirmed
		,ISNULL(pu.Unconfirmed, 0) + ISNULL(pu.Confirmed, 0) AS PoolPending
		,b.PendingWithdraw AS PendingWithdraw
		,a.Address AS Address
		,c.Status
		,c.StatusMessage
	FROM [Currency] c WITH(NOLOCK)
	LEFT JOIN [Balance] b WITH(NOLOCK) ON b.CurrencyId = c.Id and  b.UserId = @UserId
	LEFT JOIN [CryptopiaPool].[dbo].[Pool] p WITH(NOLOCK) ON p.CurrencyId = c.Id
	LEFT JOIN [CryptopiaPool].[dbo].[PoolUserInfo] pu WITH(NOLOCK) ON pu.PoolId = p.Id AND pu.UserId = @UserId
	LEFT JOIN [Address] a WITH(NOLOCK) ON a.CurrencyId = c.Id AND a.UserId = b.UserId
	WHERE c.IsEnabled = 1
	ORDER BY c.Name

RETURN
GO


