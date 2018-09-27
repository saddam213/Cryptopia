namespace Cryptopia.Common
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
	}
}
