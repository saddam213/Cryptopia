CREATE PROCEDURE [dbo].[CreateWithdraw]
      @UserId UNIQUEIDENTIFIER
	, @CurrencyId INT
    , @Amount DECIMAL(38,8)
	, @Address NVARCHAR(128)
	, @Fee DECIMAL(38,8)
	, @TwoFactorToken NVARCHAR(MAX)
	, @Status INT
	, @IsApi BIT
AS  
	BEGIN TRANSACTION

	 -- Insert the withdraw record
    INSERT INTO [dbo].[Withdraw]
           ([UserId]
           ,[CurrencyId]
           ,[Address]
           ,[Amount]
           ,[TxId]
           ,[Confirmations]
           ,[WithdrawTypeId]
           ,[WithdrawStatusId]
           ,[TimeStamp]
           ,[Fee]
		   ,[TwoFactorToken]
		   ,[IsApi])
     VALUES
           (@UserId
           ,@CurrencyId
           ,@Address
           ,@Amount
           ,''
           ,0 
           ,0 -- Normal
           ,@Status -- Unconfirmed
           ,GETDATE()
           ,@Fee
		   ,@TwoFactorToken
		   ,@IsApi)

	 IF @@ERROR <> 0
	 BEGIN
		ROLLBACK
		RAISERROR ('Error creating withdraw record.', 16, 1)
		RETURN
	 END

	 EXEC AuditUserBalance @UserId, @CurrencyId

	 IF @@ERROR <> 0
	 BEGIN
		ROLLBACK
		RAISERROR ('Error updating balance.', 16, 1)
		RETURN
	 END

	 SELECT CAST(SCOPE_IDENTITY() as int)
COMMIT
GO