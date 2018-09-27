namespace Cryptopia.IntegrationService
{
	public interface IExchangeMarket
	{
		string TradePair { get; }
		decimal Bid { get; }
		decimal Ask { get; }
		decimal Last { get; }
		decimal Volume { get; }
		decimal BaseVolume { get; }
		string MarketUrl { get; }
	}
}
