using System.Collections.Generic;

namespace Cryptopia.Common.Extensions
{
	public static class LottoHelpers
	{
		public static List<decimal> GetPrizeWeights(int prizeCount)
		{
			decimal remaining = 0;
			decimal prize = 100m;
			var weights = new List<decimal>();
			for (int i = 0; i < prizeCount; i++)
			{
				prize = prize / 2;
				weights.Add(prize);
				if (i == prizeCount - 1)
					remaining = prize;
			}

			for (int i = prizeCount > 2 ? 1 : 0; i < weights.Count; i++)
			{
				weights[i] += remaining / (prizeCount > 2 ? prizeCount - 1 : prizeCount);
			}

			return weights;
		}
	}
}
