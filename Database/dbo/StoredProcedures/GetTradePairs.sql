CREATE PROCEDURE [dbo].[GetTradePairs]
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT 
	       t.Id AS TradePairId 
         , c1.Id AS CurrencyId
		 , c1.Name as Name 
         , c1.Symbol AS Symbol 
         , c2.Id AS BaseCurrencyId 
		 , c2.Name AS BaseName 
         , c2.Symbol AS BaseSymbol 
		 , c2.TradeFee AS BaseFee
		 , t.LastTrade AS LastTrade
		 , t.Change AS Change
		 , t.StatusId as [Status]
		 , t.StatusMessage AS StatusMessage
		 , c2.MinBaseTrade AS BaseMinTrade
	FROM [dbo].[TradePair] t
	JOIN [dbo].[Currency] c1 ON t.CurrencyId1 = c1.Id
	JOIN [dbo].[Currency] c2 ON t.CurrencyId2 = c2.Id
	
RETURN
GO


