using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Base.Extensions
{
	public static class PoolExtensions
	{
		public static string TargetDifficultyToOption(double difficulty)
		{
			var diff = (int)difficulty;
			switch (diff)
			{
				case -3:
					return Constant.POOL_DIFFICULTY_OPTIONS[-3];
				case -2:
					return Constant.POOL_DIFFICULTY_OPTIONS[-2];
				case -1:
					return Constant.POOL_DIFFICULTY_OPTIONS[-1];
				default:
					break;
			}
			return Constant.POOL_DIFFICULTY_OPTIONS[0];
		}

		public static double OptionToTargetDifficulty(string option, double difficulty)
		{
			switch (option)
			{
				case Constant.POOL_DIFFICULTY_OPTION_VARDIFFHIGH:
					return -3;
				case Constant.POOL_DIFFICULTY_OPTION_VARDIFFMEDIUM:
					return -2;
				case Constant.POOL_DIFFICULTY_OPTION_VARDIFFLOW:
					return -1;
				default:
					break;
			}
			return difficulty;
		}

		public static double CalculateHashrate(double shares, AlgoType algoType, double hashRateCalculationPeriod)
		{
			var multiplier = GetHashrateMultiplier(algoType);
			var diffMultiplier = Math.Pow(2, 32) / multiplier;
			var hashrate = (diffMultiplier * shares / hashRateCalculationPeriod);
			if (hashrate > 0)
			{
				return Math.Round(hashrate, 2);
			}
			return 0.0;
		}

		public static double GetHashrateMultiplier(AlgoType algoType)
		{
			switch (algoType)
			{
				case AlgoType.Scrypt_jane:
				case AlgoType.Scrypt_og:
				case AlgoType.Scrypt_n:
				case AlgoType.Scrypt:
				case AlgoType.NeoScrypt:
					return Math.Pow(2, 16);
				case AlgoType.Groestl:
				case AlgoType.Keccak:
				case AlgoType.Fugue:
					return Math.Pow(2, 8);
				case AlgoType.Blake256:
				case AlgoType.Quark:
				case AlgoType.SHA256:
				case AlgoType.X11:
				case AlgoType.X13:
				case AlgoType.X15:
				case AlgoType.M7M:
				case AlgoType.Qubit:
				case AlgoType.Yescrypt:
				case AlgoType.C11:
				case AlgoType.CryptoNight:
				case AlgoType.Nist5:
				case AlgoType.Skein:
				case AlgoType.SHA1:
				case AlgoType.SHA2:
				case AlgoType.SHA3:
				case AlgoType.Lyra2RE:
				case AlgoType.Shavite3:
				case AlgoType.Hefty1:
					return 1;
				default:
					break;
			}
			return 1;
		}

		public static double GetShareMultiplier(AlgoType algoType)
		{
			switch (algoType)
			{
				case AlgoType.Scrypt_jane:
				case AlgoType.Scrypt_og:
				case AlgoType.Scrypt_n:
				case AlgoType.Scrypt:
				case AlgoType.NeoScrypt:
					return Math.Pow(2, 16);
				case AlgoType.Groestl:
				case AlgoType.Keccak:
				case AlgoType.Fugue:
				case AlgoType.Blake256:
					return Math.Pow(2, 8);
				case AlgoType.Quark:
				case AlgoType.SHA256:
				case AlgoType.X11:
				case AlgoType.X13:
				case AlgoType.X15:
				case AlgoType.M7M:
				case AlgoType.Qubit:
				case AlgoType.Yescrypt:
				case AlgoType.C11:
				case AlgoType.CryptoNight:
				case AlgoType.Nist5:
				case AlgoType.Skein:
				case AlgoType.SHA1:
				case AlgoType.SHA2:
				case AlgoType.SHA3:
				case AlgoType.Lyra2RE:
				case AlgoType.Shavite3:
				case AlgoType.Hefty1:
					return 1;
				default:
					break;
			}
			return 1;
		}

		public static double GetStatisticMultiplier(AlgoType algoType)
		{
			switch (algoType)
			{
				case AlgoType.Qubit:
				case AlgoType.Quark:
				case AlgoType.Blake256:
					return 256.0;
				case AlgoType.Scrypt:
				case AlgoType.SHA256:
				case AlgoType.X11:
				case AlgoType.X13:
				case AlgoType.X15:
				case AlgoType.Scrypt_n:
				case AlgoType.M7M:
				case AlgoType.Yescrypt:
				case AlgoType.NeoScrypt:
				case AlgoType.Groestl:
				case AlgoType.C11:
				case AlgoType.CryptoNight:
				case AlgoType.Keccak:
				case AlgoType.Nist5:
				case AlgoType.Skein:
				case AlgoType.SHA1:
				case AlgoType.SHA2:
				case AlgoType.SHA3:
				case AlgoType.Lyra2RE:
				case AlgoType.Scrypt_jane:
				case AlgoType.Scrypt_og:
				case AlgoType.Fugue:
				case AlgoType.Shavite3:
				case AlgoType.Hefty1:
					return 1.0;
				default:
					break;
			}
			return 1.0;
		}

		public static double CalculateEstimatedShares(double difficulty, AlgoType algoType)
		{
			var shareStatisticMultiplier = GetStatisticMultiplier(algoType);
			return Math.Round(GetShareMultiplier(algoType) * difficulty, 0) / shareStatisticMultiplier;
		}

		public static double GetBlockProgress(double estimatedShares, double currentShares)
		{
			if (estimatedShares > 0 && currentShares > 0)
			{
				return Math.Round((currentShares / estimatedShares) * 100.0, 2);
			}
			return 0.0;
		}

		public static int GetEstimatedBlockTime(double expectedShares, double statShares, double hashratePeriod)
		{
			if (statShares >= expectedShares)
				return 0;

			var statSize = expectedShares / statShares;
			return (int)(statSize * hashratePeriod);
		}

		public static double GetEstimatedNetworkHashrate(AlgoType algoType, double difficulty, int blocktime)
		{
			var estShares = CalculateEstimatedShares(difficulty, algoType);
			return CalculateHashrate(estShares, algoType, blocktime);
		}

		public static double GetEfficiency(double validShares, double invalidShares)
		{
			if (validShares > 0 && invalidShares > 0)
			{
				return 100.0 - ((invalidShares / validShares) * 100.0);
			}
			else if (validShares > 0 && invalidShares == 0)
			{
				return 100;
			}
			return 0;
		}


		public static string GetHashrateLabel(double hashrate)
		{
			double kilohash = 1000;
			double megahash = kilohash * 1000;
			double gigahash = megahash * 1000;
			double terahash = gigahash * 1000;
			double petahash = terahash * 1000;
			double exahash = petahash * 1000;
			if ((hashrate >= 0) && (hashrate < kilohash))
			{
				return hashrate.ToString("F2") + " H/s";
			}
			else if ((hashrate >= kilohash) && (hashrate < megahash))
			{
				return (hashrate / kilohash).ToString("F2") + " KH/s";
			}
			else if ((hashrate >= megahash) && (hashrate < gigahash))
			{
				return (hashrate / megahash).ToString("F2") + " MH/s";
			}
			else if ((hashrate >= gigahash) && (hashrate < terahash))
			{
				return (hashrate / gigahash).ToString("F2") + " GH/s";
			}
			else if (hashrate >= terahash && (hashrate < petahash))
			{
				return (hashrate / terahash).ToString("F2") + " TH/s";
			}
			else if (hashrate >= petahash && (hashrate < exahash))
			{
				return (hashrate / petahash).ToString("F2") + " PH/s";
			}
			else if (hashrate >= exahash)
			{
				return (hashrate / exahash).ToString("F2") + " EH/s";
			}

			return hashrate.ToString("F2") + " H/s";
		}
	
	}
}
