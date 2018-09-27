CREATE PROCEDURE [dbo].[GetUserTradeBalances]
	@UserId UNIQUEIDENTIFIER
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    SELECT 
		 c.Symbol AS Symbol
		,b.Total - (b.Unconfirmed + b.HeldForTrades + b.PendingWithdraw) AS Available
		,b.HeldForTrades AS HeldForTrades
	FROM [Currency] c WITH(NOLOCK)
	LEFT JOIN [Balance] b WITH(NOLOCK) ON b.CurrencyId = c.Id and  b.UserId = @UserId
	WHERE c.IsEnabled = 1
	ORDER BY c.Symbol

RETURN
GO


