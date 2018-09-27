CREATE PROCEDURE [dbo].[WalletGetPendingWithdraws]
AS  
    SET NOCOUNT ON;

	BEGIN TRANSACTION

    DECLARE @Processing TABLE
	(
	    Id INT,
		UserId UNIQUEIDENTIFIER,
		CurrencyId INT,
		Address NVARCHAR(128),
		Amount DECIMAL(38,8)
	);

	UPDATE Withdraw
	SET WithdrawStatusId = 1 --Processing
	OUTPUT inserted.Id,
	       inserted.UserId,
		   inserted.CurrencyId,
		   inserted.Address,
		   inserted.Amount
	INTO @Processing
	WHERE WithdrawStatusId = 0 -- Pending
	AND NULLIF(TxId, '') IS NULL; --saftey first :)

	-- if any other errors stop here and rollback
	IF @@ERROR <> 0
	 BEGIN
		ROLLBACK
		RAISERROR ('Error fetching pending withdrawals', 16, 1)
		RETURN
	 END

	 SELECT * FROM @Processing
	
COMMIT
GO



