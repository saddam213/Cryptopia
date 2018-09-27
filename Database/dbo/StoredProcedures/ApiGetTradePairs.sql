CREATE PROCEDURE [dbo].[ApiGetTradePairs]
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	SELECT
	 tp.Id,
	 c1.Symbol + '/' + c2.Symbol as Label,
	 c1.Name as Currency, 
	 c1.Symbol as Symbol,
	 c2.Name as BaseCurrency,
	 c2.Symbol as BaseSymbol,
	 ts.Name as [Status],
	 tp.StatusMessage as StatusMessage,
	 c2.TradeFee as TradeFee,
	 c1.MinTradeAmount as MinimumTrade,
	 c1.MaxTradeAmount as MaximumTrade,
	 c2.MinBaseTrade as MinimumBaseTrade,
	 c2.MaxTradeAmount as MaximumBaseTrade
	FROM TradePair tp WITH(NOLOCK)
	JOIN Currency c1 WITH(NOLOCK) on c1.Id = tp.CurrencyId1
	JOIN Currency c2 WITH(NOLOCK) on c2.Id = tp.CurrencyId2
	JOIN TradePairStatus ts WITH(NOLOCK) on ts.Id = tp.StatusId
	ORDER BY c2.Rank, c1.Symbol
	
RETURN

GO



