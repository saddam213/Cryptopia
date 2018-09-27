using System.Runtime.Serialization;

namespace Cryptopia.WalletAPI.DataObjects
{
	[DataContract]
	public class PrivateKeyData
	{
		[DataMember(Name = "Private Key: ")]
		public string PrivateKey { get; set; }

		[DataMember(Name = "WARNING:")]
		public string Warning { get; set; }
	}
}
