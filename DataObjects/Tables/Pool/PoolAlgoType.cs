namespace Cryptopia.API.DataObjects
{
	public enum PoolAlgoType : byte
	{
		Scrypt = 0,
		SHA256 = 1,
		X11 = 2,
		X13 = 3,
		X15 = 4,
		Scrypt_n = 5,
		Quark = 6,
		M7M = 7,
		Qubit = 8,
		Yescrypt = 9,
		NeoScrypt = 10,
		Groestl = 11,
		C11 = 12,
		CryptoNight = 13,
		Keccack = 14,
		Nist5 = 15,
		Skein = 16,
		SHA1 = 17,
		SHA2 = 18,
		SHA3 = 19,
		Lyra2RE = 20,
		Blake256 = 21,
		POS = 100,
		Other = 101,
		All = 102,
	}
}
