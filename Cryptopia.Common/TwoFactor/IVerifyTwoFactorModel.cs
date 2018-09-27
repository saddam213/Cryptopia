using Cryptopia.Enums;

namespace Cryptopia.Common.TwoFactor
{
	public interface IVerifyTwoFactorModel
	{
		string Answer1 { get; set; }
		string Answer2 { get; set; }
		string Code1 { get; }
		string Code2 { get; }
		string EmailCode { get; set; }
		string GoogleCode { get; set; }
		string Password { get; set; }
		string Pin { get; set; }
		string Question1 { get; set; }
		string Question2 { get; set; }
		TwoFactorType Type { get; set; }
		string UnlockSummary { get; }
	}
}
