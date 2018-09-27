CREATE PROCEDURE [dbo].[WalletUpdateWithdraw]
      @TxId nvarchar(max),
	  @Confirmations int
AS  
	BEGIN TRANSACTION
 
	UPDATE Withdraw
	SET Confirmations = @Confirmations
	WHERE TxId = @TxId -- Pending
	AND Confirmations <> @Confirmations
	
	-- if any other errors stop here and rollback
	IF @@ERROR <> 0
	 BEGIN
		ROLLBACK
		RAISERROR ('Error updating withdraw confirmations', 16, 1)
		RETURN
	 END
	
COMMIT

GO



