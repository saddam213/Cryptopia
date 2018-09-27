using System.Runtime.Serialization;

namespace AdmintopiaService.DataObjects
{
	[DataContract]
	public class Wallet
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Host { get; set; }

		[DataMember]
		public int Port { get; set; }

		[DataMember]
		public string User { get; set; }

		[DataMember]
		public string Pass { get; set; }
	}
}