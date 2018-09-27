using System;

namespace Cryptopia.OutboundService.Implementation
{
	/// <summary>
	/// Class for database query results
	/// </summary>
	public class WithdrawResult
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the currency identifier.
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public decimal Amount { get; set; }
    }
}
