using System.Runtime.Serialization;

namespace Cryptopia.WalletAPI.DataObjects
{
	/// <summary>
	/// Validate address rpc data response object
	/// </summary>
	[DataContract]
    public class ValidateAddressData
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "isvalid")]
        public bool IsValid { get; set; }
    }
}
