using Cryptopia.Entity.Auditing;
using Cryptopia.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class UserTwoFactor : IAuditable
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the user identifier.
		public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the type of two factor (Email, Phone etc)
        /// </summary>
        [Auditable(false, false)]
        public TwoFactorType Type { get; set; }

        /// <summary>
        /// Gets or sets the component the two factor is protecting (Login, Withdraw etc).
        /// </summary>
        [Auditable(false, true)]
        public TwoFactorComponent Component { get; set; }

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		public string Data { get; set; }

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		public string Data2 { get; set; }

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		public string Data3 { get; set; }

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		public string Data4 { get; set; }

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		public string Data5 { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is enabled.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
		/// </value>
		public bool IsEnabled { get; set; }

		/// <summary>
		/// Gets or sets the user.
		/// </summary>
		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }


		public void ClearData()
		{
			Data = string.Empty;
			Data2 = string.Empty;
			Data3 = string.Empty;
			Data4 = string.Empty;
			Data5 = string.Empty;
		}
	}
}