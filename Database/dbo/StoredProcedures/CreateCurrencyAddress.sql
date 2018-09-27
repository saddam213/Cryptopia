CREATE PROCEDURE [dbo].[CreateCurrencyAddress]
	  @CurrencyId INT
    , @UserId UNIQUEIDENTIFIER
    , @AddressHash NVARCHAR(MAX)
    , @PrivateKey NVARCHAR(MAX)
AS  
    SET NOCOUNT ON;

	BEGIN TRANSACTION

    INSERT INTO [dbo].[Address]
           ([UserId]
           ,[CurrencyId]
           ,[Address]
           ,[PrivateKey])
    VALUES
           (@UserId
           ,@CurrencyId
           ,@AddressHash
           ,@PrivateKey)
       
	IF @@ERROR <> 0
	BEGIN
		ROLLBACK
		RAISERROR ('Error in creating user address.', 16, 1)
		RETURN
	END

	INSERT INTO [dbo].[Balance]
           ([UserId]
           ,[CurrencyId]
           ,[Total]
		   ,[Unconfirmed]
           ,[HeldForTrades]
		   ,[PendingWithdraw])
     VALUES
           (@UserId
           ,@CurrencyId
           ,0
		   ,0
           ,0
		   ,0)


	IF @@ERROR <> 0
	BEGIN
		ROLLBACK
		RAISERROR ('Error in creating user balance.', 16, 1)
		RETURN
	END

COMMIT
GO



