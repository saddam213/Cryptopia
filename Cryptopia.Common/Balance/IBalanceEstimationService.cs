using System.Threading.Tasks;

namespace Cryptopia.Common.Balance
{
	public interface IBalanceEstimationService
	{
		Task<decimal> GetNZDPerCoin(int currencyId);
		Task<decimal> GetEstimatedBTC(decimal amount, int currencyId);
		Task<decimal> GetEstimatedNZD(decimal amount, int currencyId);
		Task<decimal> GetEstimatedValue(decimal amount, int currencyId, int baseCurrency);
		Task<decimal> GetEstimatedAmount(decimal btcAmount, int currencyId);
	}
}
