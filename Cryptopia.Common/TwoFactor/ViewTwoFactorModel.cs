using Cryptopia.Enums;

namespace Cryptopia.Common.TwoFactor
{
	public class ViewTwoFactorModel
	{
		public TwoFactorType Type { get; set; }
		public TwoFactorComponent ComponentType { get; set; }
	}
}