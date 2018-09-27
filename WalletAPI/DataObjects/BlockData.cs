using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Cryptopia.WalletAPI.DataObjects
{
	[DataContract]
	public class BlockData
	{
		[DataMember(Name = "hash")]
		public string BlockHash { get; set; }

		[DataMember(Name = "confirmations")]
		public int Confirmations { get; set; }

		[DataMember(Name = "height")]
		public int Height { get; set; }

		[DataMember(Name = "merkleroot")]
		public string TxId { get; set; }

		[DataMember(Name = "difficulty")]
		public double Difficulty { get; set; }

		[DataMember(Name = "time")]
		public string Time { get; set; }

		public DateTime Timestamp
		{
			get
			{
				if (!string.IsNullOrEmpty(Time))
				{
					if (Time.Contains(':'))
					{
						var dateTime = DateTime.MinValue;
						if (DateTime.TryParse(Time.Replace(" UTC", ""), out dateTime))
						{
							return dateTime;
						}
					}
					else
					{
						uint unix = 0;
						if (uint.TryParse(Time, out unix))
						{
							return new DateTime(1970, 1, 1).AddSeconds(unix);
						}
					}
				}
				return DateTime.MinValue;
			}
		}

		[DataMember(Name = "previousblockhash")]
		public string PreviousBlockHash { get; set; }

		[DataMember(Name = "nextblockhash")]
		public string NextBlockHash { get; set; }

		[DataMember(Name = "nonce")]
		public long Nonce { get; set; }

		[DataMember(Name = "bits")]
		public string Bits { get; set; }

		[DataMember(Name = "size")]
		public int Size { get; set; }

		[DataMember(Name = "version")]
		public string Version { get; set; }

		[DataMember(Name = "tx")]
		public List<string> Transactions { get; set; }
	}


}
