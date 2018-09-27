CREATE LOGIN [dataservice] WITH PASSWORD=N'Lm8D0Sj8mKFfRyvj4ZL1uZsmNdFf35MRwcdusXHMSQk=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

CREATE LOGIN [tradeservice] WITH PASSWORD=N'XO2KT9j/GO88m++HyJlQeU5shfTU1CN+Ufirmpi/ee0=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

CREATE LOGIN [walletinboundservice] WITH PASSWORD=N'KtoN2IOjCUc8Ab1ummi+F9yPhkIl2ItBeW44QHhuXUk=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

CREATE LOGIN [walletoutboundservice] WITH PASSWORD=N'rEw3hOB0322k7EWEsFXOEdYvZy80FOfHdijlu3DFJbQ=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO


USE [Cryptopia]
GO
/****** Object:  User [dataservice]    Script Date: 10/31/2014 16:36:25 ******/
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'dataservice')
CREATE USER [dataservice] FOR LOGIN [dataservice] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [tradeservice]    Script Date: 10/31/2014 16:36:25 ******/
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'tradeservice')
CREATE USER [tradeservice] FOR LOGIN [tradeservice] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [walletinboundservice]    Script Date: 10/31/2014 16:36:25 ******/
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'walletinboundservice')
CREATE USER [walletinboundservice] FOR LOGIN [walletinboundservice] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [walletoutboundservice]    Script Date: 10/31/2014 16:36:25 ******/
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'walletoutboundservice')
CREATE USER [walletoutboundservice] FOR LOGIN [walletoutboundservice] WITH DEFAULT_SCHEMA=[dbo]
GO



GRANT SELECT ON [dbo].[Balance] TO [tradeservice] AS [dbo]
GO
GRANT SELECT ON [dbo].[TradePair] TO [tradeservice] AS [dbo]
GO
GRANT SELECT ON [dbo].[Currency] TO [tradeservice] AS [dbo]
GO
GRANT SELECT ON [dbo].[Currency] TO [walletinboundservice] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Currency] TO [walletinboundservice] AS [dbo]
GO
GRANT SELECT ON [dbo].[Currency] TO [walletoutboundservice] AS [dbo]
GO

GRANT INSERT ON [dbo].[Deposit] TO [walletinboundservice] AS [dbo]
GO
GRANT SELECT ON [dbo].[Deposit] TO [walletinboundservice] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Deposit] TO [walletinboundservice] AS [dbo]
GO

GRANT INSERT ON [dbo].[ErrorLog] TO [walletinboundservice] AS [dbo]
GO
GRANT INSERT ON [dbo].[ErrorLog] TO [walletoutboundservice] AS [dbo]
GO
GRANT INSERT ON [dbo].[ErrorLog] TO [dataservice] AS [dbo]
GO

GRANT INSERT ON [dbo].[Trade] TO [tradeservice] AS [dbo]
GO
GRANT SELECT ON [dbo].[Trade] TO [tradeservice] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Trade] TO [tradeservice] AS [dbo]
GO
GRANT INSERT ON [dbo].[TradeHistory] TO [tradeservice] AS [dbo]
GO
GRANT SELECT ON [dbo].[TradeHistory] TO [tradeservice] AS [dbo]
GO
GRANT SELECT ON [dbo].[TradePair] TO [tradeservice] AS [dbo]
GO
GRANT SELECT ON [dbo].[Withdraw] TO [walletoutboundservice] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Withdraw] TO [walletoutboundservice] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[AuditUserBalance] TO [dataservice] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[AuditUserBalance] TO [walletinboundservice] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[AuditUserBalance] TO [walletoutboundservice] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[AuditUserBalance] TO [tradeservice] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[WalletGetPendingWithdraws] TO [walletoutboundservice] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[WalletSetWithdrawTxId] TO [walletoutboundservice] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[WalletUpdateWithdraw] TO [walletoutboundservice] AS [dbo]
GO
GRANT EXECUTE ON [dbo].[AuditUserBalance] TO [tradeservice] AS [dbo]
GO
GRANT SELECT ON [dbo].[User] TO [tradeservice] AS [dbo]
GO

GRANT EXECUTE ON [dbo].[WalletGetPendingWithdraws] TO [walletoutboundservice]
GRANT EXECUTE ON [dbo].[WalletSetWithdrawTxId] TO [walletoutboundservice]
GRANT EXECUTE ON [dbo].[WalletUpdateWithdraw] TO [walletoutboundservice]
GRANT EXECUTE ON [dbo].[AuditUserBalance] TO [walletoutboundservice]




