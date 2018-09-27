using System.ComponentModel.DataAnnotations;
using Cryptopia.Common.TwoFactor;
using Cryptopia.Common.Validation;
using Cryptopia.Enums;

namespace Web.Site.Models
{
	public class VerifyLoginTwoFactorModel : IVerifyTwoFactorModel
	{
		public TwoFactorType Type { get; set; }

		[RequiredIf("Type", TwoFactorType.EmailCode)]
		[Display(Name = nameof(Cryptopia.Resources.Authorization.twoFactorEmailCodeLabel), ResourceType = typeof(Cryptopia.Resources.Authorization))]
		public string EmailCode { get; set; }

		[RequiredIf("Type", TwoFactorType.GoogleCode)]
		[Display(Name = nameof(Cryptopia.Resources.Authorization.twoFactorGoogleCodeLabel), ResourceType = typeof(Cryptopia.Resources.Authorization))]
		public string GoogleCode { get; set; }

		[RequiredIf("Type", TwoFactorType.CryptopiaCode)]
		[Display(Name = nameof(Cryptopia.Resources.Authorization.twoFactorCryptopiaCodeLabel), ResourceType = typeof(Cryptopia.Resources.Authorization))]
		public string CryptopiaCode { get; set; }

		[RequiredIf("Type", TwoFactorType.PinCode)]
		[Display(Name = nameof(Cryptopia.Resources.Authorization.twoFactorPinLabel), ResourceType = typeof(Cryptopia.Resources.Authorization))]
		public string Pin { get; set; }

		[RequiredIf("Type", TwoFactorType.Password)]
		[Display(Name = nameof(Cryptopia.Resources.Authorization.twoFactorPasswordLabel), ResourceType = typeof(Cryptopia.Resources.Authorization))]
		public string Password { get; set; }

		[Display(Name = nameof(Cryptopia.Resources.Authorization.twoFactorQuestion1Label), ResourceType = typeof(Cryptopia.Resources.Authorization))]
		public string Question1 { get; set; }

		[Display(Name = nameof(Cryptopia.Resources.Authorization.twoFactorQuestion2Label), ResourceType = typeof(Cryptopia.Resources.Authorization))]
		public string Question2 { get; set; }

		[RequiredIf("Type", TwoFactorType.Question)]
		[Display(Name = nameof(Cryptopia.Resources.Authorization.twoFactorAnswer1Label), ResourceType = typeof(Cryptopia.Resources.Authorization))]
		public string Answer1 { get; set; }

		[RequiredIf("Type", TwoFactorType.Question)]
		[Display(Name = nameof(Cryptopia.Resources.Authorization.twoFactorAnswer2Label), ResourceType = typeof(Cryptopia.Resources.Authorization))]
		public string Answer2 { get; set; }

		public string Code1
		{
			get
			{
				switch (Type)
				{
					case TwoFactorType.EmailCode:
						return EmailCode;
					case TwoFactorType.GoogleCode:
						return GoogleCode;
					case TwoFactorType.CryptopiaCode:
						return CryptopiaCode;
					case TwoFactorType.PinCode:
						return Pin;
					case TwoFactorType.Password:
						return Password;
					case TwoFactorType.Question:
						return Answer1;
					default:
						break;
				}
				return string.Empty;
			}
		}

		public string Code2
		{
			get
			{
				return Answer2;
			}
		}

		public string UnlockSummary
		{
			get
			{
				switch (Type)
				{
					case TwoFactorType.None:
						break;
					case TwoFactorType.EmailCode:
						return Cryptopia.Resources.Authorization.twoFactorEmailSummary;
					case TwoFactorType.GoogleCode:
						return Cryptopia.Resources.Authorization.twoFactorGoogleSummary;
					case TwoFactorType.CryptopiaCode:
						return Cryptopia.Resources.Authorization.twoFactorCryptopiaSummary;
					case TwoFactorType.PinCode:
						return Cryptopia.Resources.Authorization.twoFactorPinSummary;
					case TwoFactorType.Password:
						return Cryptopia.Resources.Authorization.twoFactorPasswordSummary;
					case TwoFactorType.Question:
						return Cryptopia.Resources.Authorization.twoFactorQuestionSummary;
					default:
						break;
				}
				return string.Empty;
			}
		}
	}
}