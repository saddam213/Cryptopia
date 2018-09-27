using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cryptopia.Enums
{
	public static class Constant
	{

		public const int BITCOIN_ID = 1;
		public const int DOTCOIN_ID = 2;
		public const int LITECOIN_ID = 3;
		public const int USDT_ID = 469;
		public const int NZDT_ID = 514;
        public const int CEFS_ID = 664;

        public const int DEFAULT_QUEUE_ID = 1;

		public const int EMOTICON_MAXHEIGHT = 80;
		public const int EMOTICON_MAXWIDTH = 240;
		public const long EMOTICON_MAXSIZE = 262144; //256kb

		public const decimal REFERRAL_ACTIVEBONUS = 100m;
		public const decimal REFERRAL_TRADEPERCENT = 10m;

		public const decimal VERIFICATION_WITHDRAW_LEVEL1_LIMIT = 5000m;
		public const decimal VERIFICATION_WITHDRAW_LEVEL2_LIMIT = 50000m;
		public const decimal VERIFICATION_WITHDRAW_LEVEL3_LIMIT = 500000m;
		public const decimal VERIFICATION_WITHDRAW_LEVEL4_LIMIT = 500000m;
		public const int VERIFICATION_MAX_WIDTH = 500;
		public const int VERIFICATION_MAX_HEIGHT = 500;

		public const int MARKETPLACE_CATEGORY_ADULT = 99;

		public static readonly string NzdtBaseExchangeAddress = "3PEVMYNfSNhtsCchTbeig5gmhPXBzJ2E3fB";

		public static readonly Regex NZDT_BANK_REGEX = new Regex(@"\b[0-9]{2}-?[0-9]{4}-?[0-9]{7}-?[0-9]{2,3}\b");

		public static readonly Guid SYSTEM_USER = new Guid("6BECEDFE-0746-47CE-914E-886F8564B643");
		public static readonly Guid SYSTEM_USER_VOTE = new Guid("AFF2B117-7EB1-4196-BA62-FE300BE00B48");
		public static readonly Guid SYSTEM_USER_FAUCET = new Guid("f9a0d401-ca76-4f30-bb96-9728550b0ab2");
		public static readonly Guid SYSTEM_USER_CHATBOT = new Guid("74076267-259a-4acc-95b1-7c35224b193e");
		public static readonly Guid SYSTEM_USER_REWARD = new Guid("eb7c5b99-c813-441e-ae50-60f56aa38ac6");
		public static readonly Guid SYSTEM_USER_LOTTO = new Guid("D8EB3023-F5BF-49A7-9BE9-D30D332F9AEA");
		public static readonly Guid SYSTEM_USER_DUSTBIN = new Guid("7BEBF4BE-3C6B-4297-911D-8537C2426341");
		public static readonly Guid SYSTEM_USER_MINESHAFT = new Guid("D36E5D46-C519-4480-9EC2-06A9D8ADDA4A");
		public static readonly Guid SYSTEM_USER_REFERRAL = new Guid("73e435ab-9e34-4d91-8d93-cb9e307585d3");
		public static readonly Guid SYSTEM_USER_PAYTOPIA = new Guid("8DF9950D-C6E0-488A-BEB6-112DC635CD45");
		public static readonly Guid SYSTEM_USER_ESCROW = new Guid("16110DB1-298F-4684-A542-6C4E17777F88");
        public static readonly Guid SYSTEM_USER_CEFS = new Guid("C3E1C505-1D36-476C-AC85-59B6FC5E28C9");
        public static readonly Guid SYSTEM_USER_STAGING = new Guid("7FA15B35-10C7-4E6B-AADD-7D1ECBFB844F");

        public static readonly List<Guid> SYSTEM_ACCOUNTS = new List<Guid>
		{
			SYSTEM_USER,
			SYSTEM_USER_VOTE,
			SYSTEM_USER_FAUCET,
			SYSTEM_USER_CHATBOT,
			SYSTEM_USER_REWARD,
			SYSTEM_USER_LOTTO,
			SYSTEM_USER_DUSTBIN,
			SYSTEM_USER_MINESHAFT,
			SYSTEM_USER_REFERRAL,
			SYSTEM_USER_PAYTOPIA,
			SYSTEM_USER_ESCROW,
            SYSTEM_USER_CEFS
        };

		public static readonly List<string> AllowedImageExtesions = new List<string> { "image/png", "image/bmp", "image/jpg", "image/jpeg" };

		public const string POOL_DIFFICULTY_OPTION_FIXEDDIFF = "Fixed Diff";
		public const string POOL_DIFFICULTY_OPTION_VARDIFFLOW = "Var-Diff Low";
		public const string POOL_DIFFICULTY_OPTION_VARDIFFMEDIUM = "Var-Diff Medium";
		public const string POOL_DIFFICULTY_OPTION_VARDIFFHIGH = "Var-Diff High";
		public static readonly Dictionary<int, string> POOL_DIFFICULTY_OPTIONS = new Dictionary<int, string>
		{
			{-1, POOL_DIFFICULTY_OPTION_VARDIFFLOW},
			{-2, POOL_DIFFICULTY_OPTION_VARDIFFMEDIUM},
			{-3, POOL_DIFFICULTY_OPTION_VARDIFFHIGH},
			{0, POOL_DIFFICULTY_OPTION_FIXEDDIFF}
		};

		public static readonly List<AlgoType> SupportedPoolAlgos = new List<AlgoType>
		{
			AlgoType.Blake256,
			AlgoType.Fugue,
			AlgoType.Groestl,
			AlgoType.Hefty1,
			AlgoType.Keccak,
			AlgoType.Lyra2RE,
			AlgoType.NeoScrypt,
			AlgoType.Nist5,
			AlgoType.Quark,
			AlgoType.Qubit,
			AlgoType.Scrypt,
			AlgoType.Scrypt_jane,
			AlgoType.Scrypt_n,
			AlgoType.Scrypt_og,
			AlgoType.SHA256,
			AlgoType.Skein,
			AlgoType.X11,
			AlgoType.X13,
			AlgoType.X15
		};
	}

	public static class CryptopiaClaim
	{
		public const string Theme = "Cryptopia.Theme";
		public const string ChatHandle = "Cryptopia.ChatHandle";
		public const string HideZeroBalance = "Cryptopia.HideZeroBalance";
		public const string ShowFavoriteBalance = "Cryptopia.ShowFavoriteBalance";
		public const string Shareholder = "Cryptopia.Shareholder";
		public const string ChartSettings = "Cryptopia.ChartSettings";
	}
}
