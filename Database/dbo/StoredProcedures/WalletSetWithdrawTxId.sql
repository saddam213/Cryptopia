CREATE PROCEDURE [dbo].[WalletSetWithdrawTxId]
      @WithdrawId INT,
	  @TxId nvarchar(256)
AS  

BEGIN TRANSACTION

	UPDATE Withdraw
	SET TxId = @TxId
	   ,WithdrawStatusId = 2 --complete
	WHERE Id = @WithdrawId

	-- if any other errors stop here and rollback
	IF @@ERROR <> 0
	 BEGIN
		ROLLBACK
		RAISERROR ('Error fetching pending withdrawals', 16, 1)
		RETURN
	 END
	
COMMIT
GO



