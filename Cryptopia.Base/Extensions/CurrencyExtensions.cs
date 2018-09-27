using System;

namespace Cryptopia.Base
{
	public static class CurrencyExtensions
	{
		/// <summary>
		/// Gets the fees.
		/// </summary>
		/// <param name="amount">The amount.</param>
		/// <param name="fee">The fee.</param>
		/// <returns></returns>
		public static decimal GetFees(this decimal amount, decimal fee)
		{
			return decimal.Round(decimal.Multiply(decimal.Divide(amount, 100m), fee), 8);
		}

		/// <summary>
		/// Includings the fees.
		/// </summary>
		/// <param name="amount">The amount.</param>
		/// <param name="fee">The fee.</param>
		/// <returns></returns>
		public static decimal IncludingFees(this decimal amount, decimal fee)
		{
			return decimal.Round(decimal.Add(amount, amount.GetFees(fee)), 8);
		}

		/// <summary>
		/// Excludings the fees.
		/// </summary>
		/// <param name="amount">The amount.</param>
		/// <param name="fee">The fee.</param>
		/// <returns></returns>
		public static decimal ExcludingFees(this decimal amount, decimal fee)
		{
			return decimal.Round(decimal.Subtract(amount, amount.GetFees(fee)), 8);
		}

		/// <summary>
		/// Gets a percentage of a number.
		/// </summary>
		/// <param name="total">The total.</param>
		/// <param name="percentage">The percentage.</param>
		public static decimal GetPercentage(this decimal total, decimal percentage)
		{
			return decimal.Round(decimal.Divide(decimal.Multiply(total, percentage), 100), 8);
		}

		public static decimal GetDecimalFromDouble(this double value)
		{
			return decimal.Round((decimal)value, 8, MidpointRounding.AwayFromZero);
		}


		public static decimal GetPercentChanged(this decimal lastTrade, decimal newTrade)
		{
			if (lastTrade > 0)
			{
				return Math.Round(((newTrade - lastTrade) / lastTrade * 100m), 2);
			}
			return 0.00m;
		}

		public static bool IsValidPaymentId(string paymentId)
		{
			if (string.IsNullOrEmpty(paymentId))
				return true;

			if (paymentId.Length != 64)
				return false;

			bool isHex;
			foreach (var c in paymentId)
			{
				isHex = ((c >= '0' && c <= '9') ||
								 (c >= 'a' && c <= 'f') ||
								 (c >= 'A' && c <= 'F'));
				if (!isHex)
					return false;
			}
			return true;
		}
	}
}
