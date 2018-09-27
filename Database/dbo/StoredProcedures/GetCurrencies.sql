CREATE PROCEDURE [dbo].[GetCurrencies]
AS  
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
    SET NOCOUNT ON;

	SELECT DISTINCT
			c.Id AS CurrencyId 
		, c.Name AS Name 
		, c.Symbol AS Symbol 
		, c.WithdrawFee AS WithdrawFee
		, c.MinWithdraw AS WithdrawMin
		, c.MaxWithdraw AS WithdrawMax
		, c.Status as Status
		, c.IsFeatured
		, c.MinTip AS TipMin
		, c.IsTipEnabled
		, c.IsFaucetEnabled
		, c.MinFaucet AS FaucetMin
		, c.ForumId
		, c.[Version]
		, c.Connections
		, c.Errors
		, c.NetworkType
		, c.Block
		, c.MinConfirmations
		, c.PoolFee
		, c.TradeFee
		, c.WithdrawFeeType
		, ci.AlgoType
		, ci.Description
	FROM [dbo].[Currency] c WITH (NOLOCK)
	JOIN [dbo].[CurrencyInfo] ci WITH (NOLOCK) ON ci.Id = c.Id
	WHERE c.IsEnabled = 1
	
RETURN
GO