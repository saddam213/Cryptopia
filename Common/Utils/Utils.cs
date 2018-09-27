using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Cryptopia.Common.Utils
{
	public static class Utils
    {
        public static string CreateHashFromString(string value)
        {
            var sb = new StringBuilder();
            using (var hash = SHA256Managed.Create())
            {
                var enc = Encoding.UTF8;
                var result = hash.ComputeHash(enc.GetBytes(value));
                foreach (var b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
            }
            return sb.ToString();
        }

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
