namespace Cryptopia.Common.TwoFactor
{
	public class GoogleTwoFactorData
	{
		public string PrivateKey { get; set; }
		public string PublicKey { get; set; }
		public bool IsValid
		{
			get
			{
				return !string.IsNullOrEmpty(PublicKey)
				  && !string.IsNullOrEmpty(PrivateKey);
			}
		}
	}
}
