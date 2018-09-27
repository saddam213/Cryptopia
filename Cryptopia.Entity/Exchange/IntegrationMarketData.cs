using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class IntegrationMarketData
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public int IntegrationExchangeId { get; set; }
		public int TradePairId { get; set; }
		public decimal Bid { get; set; }
		public decimal Ask { get; set; }
		public decimal Last { get; set; }
		public decimal Volume { get; set; }
		public decimal BaseVolume { get; set; }

		[MaxLength(256)]
		public string MarketUrl { get; set; }
		public DateTime Timestamp { get; set; }

		[ForeignKey("TradePairId")]
		public virtual TradePair TradePair { get; set; }

		[ForeignKey("IntegrationExchangeId")]
		public virtual IntegrationExchange IntegrationExchange { get; set; }
	}
}