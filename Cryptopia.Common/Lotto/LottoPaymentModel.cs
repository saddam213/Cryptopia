using Cryptopia.Common.TwoFactor;
using Cryptopia.Common.Validation;
using Cryptopia.Enums;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Common.Lotto
{
	public class LottoPaymentModel : IVerifyTwoFactorModel
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Rate { get; set; }
		public string Symbol { get; set; }
		public decimal Balance { get; set; }

		[RangeBase(1, 200)]
		public int Amount { get; set; }

		#region Twofactor

		public TwoFactorType Type { get; set; }

		[RequiredIf("Type", TwoFactorType.EmailCode)]
		[Display(Name = nameof(Resources.Authorization.twoFactorEmailCodeLabel), ResourceType = typeof(Resources.Authorization))]
		public string EmailCode { get; set; }

		[RequiredIf("Type", TwoFactorType.GoogleCode)]
		[Display(Name = nameof(Resources.Authorization.twoFactorGoogleCodeLabel), ResourceType = typeof(Resources.Authorization))]
		public string GoogleCode { get; set; }

		[RequiredIf("Type", TwoFactorType.CryptopiaCode)]
		[Display(Name = nameof(Resources.Authorization.twoFactorCryptopiaCodeLabel), ResourceType = typeof(Resources.Authorization))]
		public string CryptopiaCode { get; set; }

		[RequiredIf("Type", TwoFactorType.PinCode)]
		[Display(Name = nameof(Resources.Authorization.twoFactorPinLabel), ResourceType = typeof(Resources.Authorization))]
		public string Pin { get; set; }

		[RequiredIf("Type", TwoFactorType.Password)]
		[Display(Name = nameof(Resources.Authorization.twoFactorPasswordLabel), ResourceType = typeof(Resources.Authorization))]
		public string Password { get; set; }

		[Display(Name = nameof(Resources.Authorization.twoFactorQuestion1Label), ResourceType = typeof(Resources.Authorization))]
		public string Question1 { get; set; }

		[Display(Name = nameof(Resources.Authorization.twoFactorQuestion2Label), ResourceType = typeof(Resources.Authorization))]
		public string Question2 { get; set; }

		[RequiredIf("Type", TwoFactorType.Question)]
		[Display(Name = nameof(Resources.Authorization.twoFactorAnswer1Label), ResourceType = typeof(Resources.Authorization))]
		public string Answer1 { get; set; }

		[RequiredIf("Type", TwoFactorType.Question)]
		[Display(Name = nameof(Resources.Authorization.twoFactorAnswer2Label), ResourceType = typeof(Resources.Authorization))]
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
						return Resources.Authorization.twoFactorEmailSummary;
					case TwoFactorType.GoogleCode:
						return Resources.Authorization.twoFactorGoogleSummary;
					case TwoFactorType.CryptopiaCode:
						return Resources.Authorization.twoFactorCryptopiaSummary;
					case TwoFactorType.PinCode:
						return Resources.Authorization.twoFactorPinSummary;
					case TwoFactorType.Password:
						return Resources.Authorization.twoFactorPasswordSummary;
					case TwoFactorType.Question:
						return Resources.Authorization.twoFactorQuestionSummary;
					default:
						break;
				}
				return string.Empty;
			}
		}

		#endregion

		public int LottoItemId { get; set; }
	}
}
