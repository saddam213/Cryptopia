using Cryptopia.Common.Currency;
using Cryptopia.Common.Pool;

namespace Cryptopia.Common.Mineshaft
{
	public class FeaturedPool
	{
		public FeaturedPool(CurrencyModel pool)
		{
			Pool = pool;
		}
		public CurrencyModel Pool { get; set; }
	}
}
