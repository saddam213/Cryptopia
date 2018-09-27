CREATE PROCEDURE [dbo].[AuditUserBalance]
      @UserId UNIQUEIDENTIFIER
	, @CurrencyId INT
AS  

	DECLARE @TotalIn DECIMAL(18,8)
	DECLARE @TotalOut DECIMAL(18,8)
	DECLARE @TotalBalance DECIMAL(18,8)
	DECLARE @TotalDepositConfirmed DECIMAL(18,8)
	DECLARE @TotalDepositUnconfirmed DECIMAL(18,8)
	DECLARE @TotalWithdraw  DECIMAL(18,8)
	DECLARE @TotalPendingWithdraw DECIMAL(18,8)
	DECLARE @TotalTransferIn  DECIMAL(18,8)
	DECLARE @TotalTransferOut DECIMAL(18,8)
	DECLARE @TotalBuy  DECIMAL(18,8) 
	DECLARE @TotalSell  DECIMAL(18,8)
	DECLARE @TotalBuyBase DECIMAL(18,8)
	DECLARE @TotalSellBase DECIMAL(18,8)
	DECLARE @TotalBuyFee  DECIMAL(18,8)
	DECLARE @TotalSellFee  DECIMAL(18,8)
	DECLARE @HeldForOrders  DECIMAL(18,8)
	DECLARE @HeldForOrdersBase  DECIMAL(18,8)
	DECLARE @HeldForOrderFee  DECIMAL(18,8)
	DECLARE @TotalHeldForOrders  DECIMAL(18,8)

	--Find all confirmed deposits for this currency
	SELECT @TotalDepositConfirmed = SUM(CONVERT(DECIMAL(18,8), Amount)) 
	FROM Deposit 
	WHERE UserId = @UserId
	AND CurrencyId = @CurrencyId
	AND DepositStatusId = 1
	--SELECT @TotalDepositConfirmed As TotalDepositConfirmed
	
	--Find all unconfirmed deposits for this currency
	SELECT @TotalDepositUnconfirmed = SUM(CONVERT(DECIMAL(18,8), Amount)) 
	FROM Deposit 
	WHERE UserId = @UserId
	AND CurrencyId = @CurrencyId
	AND DepositStatusId = 0
	--SELECT @TotalDepositUnconfirmed As TotalDepositUnconfirmed
	
	--Find all the currency the user withdrew
	SELECT @TotalWithdraw = SUM(CONVERT(DECIMAL(18,8), Amount)) 
	FROM Withdraw
	WHERE UserId = @UserId 
	AND CurrencyId = @CurrencyId 
	AND WithdrawStatusId = 2 -- complete
	--SELECT @TotalWithdraw as TotalWithdraw

	--Find all the currency the user withdrew
	SELECT @TotalPendingWithdraw = SUM(CONVERT(DECIMAL(18,8), Amount)) 
	FROM Withdraw
	WHERE UserId = @UserId
	AND CurrencyId = @CurrencyId 
	AND WithdrawStatusId NOT IN (2,5) --complete, cancelled
 	--SELECT @TotalPendingWithdraw as TotalPendingWithdraw
	
	--Find all of this currency the user brought
	SELECT @TotalBuy = SUM(CONVERT(DECIMAL(18,8), Amount)) 
	FROM TradeHistory
	WHERE UserId = @UserId
	AND CurrencyId = @CurrencyId
	--SELECT @TotalBuy as TotalBuy
	
	--Find all the currency the user sold
	SELECT @TotalSell = SUM(CONVERT(DECIMAL(18,8), Amount)) 
	FROM TradeHistory
	WHERE ToUserId = @UserId
	AND CurrencyId = @CurrencyId
	--SELECT @TotalSell as TotalSell
	
    --Find all the buy fees paid in this currency
	SELECT @TotalBuyFee = SUM(CONVERT(DECIMAL(18,8), t.Fee)) 
	FROM TradeHistory t
	JOIN TradePair tp ON tp.Id = t.TradePairId
	WHERE UserId = @UserId
	AND tp.CurrencyId2 = @CurrencyId 
	--SELECT @TotalBuyFee as TotalBuyFee
	
	 --Find all the sell fees paid in this currency
	SELECT @TotalSellFee = SUM(CONVERT(DECIMAL(18,8), t.Fee)) 
	FROM TradeHistory t
	JOIN TradePair tp ON tp.Id = t.TradePairId
	WHERE ToUserId = @UserId
	AND tp.CurrencyId2 = @CurrencyId 
	--SELECT @TotalSellFee as TotalSellFee
	
		
	--Find currency sent as payment for trade
	SELECT @TotalBuyBase = SUM(CONVERT(DECIMAL(18,8), t.Amount) * CONVERT(DECIMAL(18,8), t.Rate)) 
	FROM TradeHistory t
	JOIN TradePair tp ON tp.Id = t.TradePairId
	WHERE ToUserId = @UserId
	AND tp.CurrencyId2 = @CurrencyId 
	--SELECT @TotalBuyBase as TotalBuyBase
	
	--Find currency received as payment for trade
	SELECT @TotalSellBase = SUM(CONVERT(DECIMAL(18,8), t.Amount) * CONVERT(DECIMAL(18,8), t.Rate)) 
	FROM TradeHistory t
	JOIN TradePair tp ON tp.Id = t.TradePairId
	WHERE UserId = @UserId
	AND tp.CurrencyId2 = @CurrencyId 
	--SELECT @TotalSellBase as TotalSellBase 
	
	--Find all the orders where this currency is whats being traded and add up the amount
	SELECT @HeldForOrders = SUM(CONVERT(DECIMAL(18,8), Remaining)) 
	FROM [Trade] o
	JOIN TradePair t ON t.Id = o.TradePairId
	WHERE UserId = @UserId 
	AND o.TradeStatusId IN (0,2) 
	AND t.CurrencyId1 = @CurrencyId 
	AND o.TradeTypeId = 1
	--SELECT @HeldForOrders as HeldForOrders 

	--Find all the orders where this currency is the base and add up the Amount * Rate
	SELECT @HeldForOrdersBase = SUM((CONVERT(DECIMAL(18,8), Remaining) * CONVERT(DECIMAL(18,8), Rate)) + CONVERT(DECIMAL(18,8), Fee)) 
	FROM [Trade] o
	JOIN TradePair t ON t.Id = o.TradePairId
	WHERE UserId = @UserId 
	AND o.TradeStatusId IN (0, 2) 
	AND o.TradeTypeId = 0
	AND t.CurrencyId2 = @CurrencyId 
	--SELECT @HeldForOrdersBase as HeldForOrdersBase  

	--Get all transfers received
	SELECT @TotalTransferIn = SUM(CONVERT(decimal(18,8), t.Amount)) 
	FROM TransferHistory t
	WHERE ToUserId = @UserId  
	AND t.CurrencyId = @CurrencyId

	--Get all transfers sent
	SELECT @TotalTransferOut = SUM(CONVERT(decimal(18,8), t.Amount)) 
	FROM TransferHistory t
	WHERE UserId = @UserId 
	AND t.CurrencyId = @CurrencyId 

	SELECT @TotalIn = (ISNULL(@TotalDepositConfirmed, 0) + ISNULL(@TotalDepositUnconfirmed,0) + ISNULL(@TotalBuy,0) + ISNULL(@TotalBuyBase,0) + ISNULL(@TotalTransferIn,0))
	SELECT @TotalOut = (ISNULL(@TotalWithdraw,0) + ISNULL(@TotalSell,0) + ISNULL(@TotalSellBase,0) + ISNULL(@TotalBuyFee,0) + ISNULL(@TotalSellFee,0) + ISNULL(@TotalTransferOut, 0))-- + ISNULL(@TotalWithdrawFee,0)) 
	SELECT @TotalBalance = @TotalIn - @TotalOut
	SELECT @TotalHeldForOrders = ISNULL(@HeldForOrders,0) + (ISNULL(@HeldForOrdersBase, 0))
	--SELECT @TotalBalance as Total
	--SELECT @TotalDepositUnconfirmed as Unconfirmed
	--SELECT @TotalHeldForOrders as HeldForOrders 
	--SELECT @TotalPendingWithdraw as PendingWithdraw
	
BEGIN TRANSACTION

	--Update Balance
	UPDATE Balance 
	SET Total = ISNULL(@TotalBalance, 0)
	   ,Unconfirmed = ISNULL(@TotalDepositUnconfirmed, 0)
	   ,HeldForTrades = ISNULL(@TotalHeldForOrders, 0)
	   ,PendingWithdraw = ISNULL(@TotalPendingWithdraw, 0)
	WHERE UserId = @UserId
	AND CurrencyId = @CurrencyId

	-- If update returned no rows and there is no error, there must be no balance, create one now
	IF(@@ROWCOUNT = 0 AND @@ERROR = 0)
		BEGIN
			INSERT INTO Balance 
				([UserId]
				,[CurrencyId]
				,[Total]
				,[Unconfirmed]
				,[HeldForTrades]
				,[PendingWithdraw])
			SELECT @UserId
				,@CurrencyId
				,ISNULL(@TotalBalance, 0)
				,ISNULL(@TotalDepositUnconfirmed, 0)
				,ISNULL(@TotalHeldForOrders, 0)
				,ISNULL(@TotalPendingWithdraw, 0)
			WHERE NOT EXISTS (SELECT * FROM Balance WITH(NOLOCK) WHERE UserId = @UserId AND CurrencyId = @CurrencyId)
		END

	--If there is an error rollback
	IF @@ERROR <> 0
		BEGIN
			ROLLBACK
			RAISERROR ('Error updating user balance.', 16, 1)
			RETURN
		END
COMMIT
