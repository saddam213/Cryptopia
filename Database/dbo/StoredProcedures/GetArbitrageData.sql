CREATE PROCEDURE [dbo].[GetArbitrageData]
AS  
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT 
		tp.Id AS TradePairId,
		c1.Name AS Currency,
		ie.Name AS Exchange,
		c1.Id AS CurrencyId,
		c2.Id AS BaseCurrencyId,
		c1.Symbol AS Symbol,
		c2.Symbol AS BaseSymbol,
		imd.Ask,
		imd.Bid,
		imd.Last,
		imd.Volume,
		imd.BaseVolume,
		imd.MarketUrl
	FROM [IntegrationMarketData] imd
	JOIN [IntegrationExchange] ie ON ie.Id = imd.IntegrationExchangeId
	JOIN TradePair tp ON tp.Id = imd.TradePairId
	JOIN Currency c1 ON c1.Id = tp.CurrencyId1
	JOIN Currency c2 ON c2.Id = tp.CurrencyId2
	WHERE tp.StatusId <> 3 AND ie.IsEnabled = 1
	ORDER BY tp.Id

RETURN

GO