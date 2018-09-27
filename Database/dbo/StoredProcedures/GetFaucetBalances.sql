CREATE PROCEDURE [dbo].[GetFaucetBalances]
	@UserId UNIQUEIDENTIFIER
   ,@FaucetUserId UNIQUEIDENTIFIER
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT 
		 ISNULL(b.Total - (b.PendingWithdraw + b.HeldForTrades + b.Unconfirmed), 0) AS Balance
		,c.Name
		,c.Symbol
		,c.MinFaucet AS Payout
		,c.Id AS CurrencyId
		,ISNULL(f.Timestamp, '2000-01-01 00:00:00.1') AS LastPayout
	FROM Currency c
	LEFT JOIN Balance b ON b.CurrencyId = c.Id AND b.UserId = @FaucetUserId 
	LEFT JOIN FaucetClaim f ON f.CurrencyId = c.Id AND f.UserId = @UserId
	WHERE c.IsEnabled = 1 AND c.IsFaucetEnabled = 1 AND c.MinFaucet > 0

RETURN
GO


