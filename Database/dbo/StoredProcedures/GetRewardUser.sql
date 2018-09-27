CREATE PROCEDURE [dbo].[GetRewardUser]
   @Type NVARCHAR(128)
AS  
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
    SET NOCOUNT ON;

	IF(@Type = 'Tip')
		BEGIN
			SELECT TOP 1 
				u.Id AS UserId , 
				t.Id  AS LastItemId
			FROM TransferHistory t
			JOIN [User] u ON u.Id = t.UserId 
			WHERE t.TransferType = 3 AND u.DisableRewards = 0
			ORDER BY t.Id DESC
		END
	ELSE IF(@Type = 'Trade')
		BEGIN
			SELECT TOP 1 
				u.Id AS UserId, 
				t.TradePairId  AS LastItemId
			FROM TradeHistory t
			JOIN [User] u ON u.Id = t.UserId 
			WHERE t.IsApi = 0 AND u.DisableRewards = 0
			ORDER BY t.Id DESC
		END
	ELSE IF(@Type = 'Block')
		BEGIN
			DECLARE @TablePrefix NVARCHAR(128)
			DECLARE @WorkerName NVARCHAR(128)
			SELECT TOP 1 @TablePrefix = p.TablePrefix
			FROM [CryptopiaPool].[dbo].[PoolInfo] pii
			JOIN [CryptopiaPool].[dbo].[Pool] p ON p.Id = pii.PoolId
			WHERE pii.LastBlockTime is not null
			ORDER BY pii.LastBlockTime DESC
			
			EXEC (
			N'SELECT TOP 1
				pw.UserId,  
				b.Height AS LastItemId
			FROM [CryptopiaPool].[dbo].[' + @TablePrefix + '_Blocks] b
			JOIN [CryptopiaPool].[dbo].[PoolWorker] pw ON pw.Name = b.WorkerName
			JOIN [Cryptopia].[dbo].[User] u on u.Id = pw.UserId
			WHERE u.DisableRewards = 0
			ORDER BY b.Id DESC')
		END
	ELSE IF(@Type = 'LastShare')
		BEGIN
			SELECT TOP 1
				u.Id AS UserId,
				pw.Id AS LastItemId
			FROM [CryptopiaPool].[dbo].[PoolWorker] pw
			JOIN [Cryptopia].[dbo].[User] u ON u.Id = pw.UserId
			WHERE u.DisableRewards = 0
			ORDER BY pw.LastShareTime DESC
		END
	ELSE IF(@Type = 'Chat')
		BEGIN
			SELECT TOP 1 
				CONVERT(UNIQUEIDENTIFIER, u.Id) AS UserId, 
				c.Id  AS LastItemId
			FROM [CryptopiaHub].[dbo].[ChatMessage] c
			JOIN [CryptopiaHub].[dbo].AspNetUsers u ON u.Id = c.UserId 
			WHERE u.DisableRewards = 0
			ORDER BY c.Id DESC
		END
RETURN
GO