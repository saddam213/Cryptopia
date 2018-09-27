using System.Runtime.Serialization;

namespace Cryptopia.WalletAPI.DataObjects
{
	[DataContract]
    public class GetInfoData
    {
        [DataMember(Name = "connections")]
        public int Connections { get; set; }

        [DataMember(Name = "blocks")]
        public int Blocks { get; set; }

        [DataMember(Name = "difficulty")]
        public double Difficulty { get; set; }

        [DataMember]
        public double Hashrate { get; set; }

		[DataMember(Name = "balance")]
		public decimal Balance { get; set; }

		[DataMember(Name = "version")]
		public string Version { get; set; }

		[DataMember(Name = "errors")]
		public string Errors { get; set; }
    }

    [DataContract]
    public class GetInfoDataPOS
    {
        [DataMember(Name = "blocks")]
        public int Blocks { get; set; }

        [DataMember(Name = "difficulty")]
        public POSDifficulty Difficulty { get; set; }

        [DataMember(Name = "connections")]
        public int Connections { get; set; }

		[DataMember(Name = "balance")]
		public decimal Balance { get; set; }

		[DataMember(Name = "version")]
		public string Version { get; set; }

		[DataMember(Name = "errors")]
		public string Errors { get; set; }
    }

    [DataContract]
    public class GetMiningInfoData
    {
        [DataMember(Name = "blocks")]
        public int Blocks { get; set; }

        [DataMember(Name = "difficulty")]
        public double Difficulty { get; set; }

        [DataMember(Name = "networkhashps")]
        public double NetworkHashrate { get; set; }
    }

    [DataContract]
    public class GetMiningInfoDataPOS
    {
        [DataMember(Name = "blocks")]
        public int Blocks { get; set; }

        [DataMember(Name = "difficulty")]
        public POSDifficulty Difficulty { get; set; }

        [DataMember(Name = "netmhashps")]
        public double NetworkHashrate { get; set; }
    }

	[DataContract(Name = "difficulty")]
	public class POSDifficulty
	{
		[DataMember(Name = "proof-of-work")]
		public double Difficulty { get; set; }
	}

	[DataContract]
	public class GetMiningInfoDataPOS2
	{
		[DataMember(Name = "Blocks")]
		public int Blocks { get; set; }

		[DataMember(Name = "Difficulty")]
		public POSDifficulty2 Difficulty { get; set; }

		[DataMember(Name = "Net MH/s")]
		public double NetworkHashrate { get; set; }
	}

 
	[DataContract(Name = "Difficulty")]
	public class POSDifficulty2
	{
		[DataMember(Name = "Proof of Work")]
		public double Difficulty { get; set; }
	}

	[DataContract]
	public class GetMiningInfoDataPOS3
	{
		[DataMember(Name = "blocks")]
		public int Blocks { get; set; }

		[DataMember(Name = "PoW difficulty")]
		public double Difficulty { get; set; }

		[DataMember(Name = "networkhashps")]
		public double NetworkHashrate { get; set; }
	}

    
    public class PeerInfo
    {
        public string addr { get; set; }
        //public string services { get; set; }
        //public int lastsend { get; set; }
        //public int lastrecv { get; set; }
        //public int bytessent { get; set; }
        //public int bytesrecv { get; set; }
        //public int conntime { get; set; }
        //public int version { get; set; }
        //public string subver { get; set; }
        //public bool inbound { get; set; }
        public int startingheight { get; set; }
        //public int banscore { get; set; }
    }
}
