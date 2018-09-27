using Cryptopia.Common.TwoFactor;
using Cryptopia.Common.Validation;
using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Transfer
{
	public class TransferCurrencyModel : IVerifyTwoFactorModel
	{
		public string Symbol { get; set; }
		public decimal Balance { get; set; }

		[RequiredBase]
		public int CurrencyId { get; set; }

		[Display(Name = nameof(Resources.User.transferAmountLabel), ResourceType = typeof(Resources.User))]
		public decimal Amount { get; set; }

		[RequiredBase]
		public string UserName { get; set; }

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
			get { return Answer2; }
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

		public string Name { get; set; }
		public string ReturnUrl { get; set; }
		public string DisplayName
		{
			get { return $"{Name}({Symbol})"; }
		}


		#endregion

		public bool HasWithdrawLimit { get; set; }
		public decimal WithdrawLimit { get; set; }
		public decimal WithdrawTotal { get; set; }
		public decimal EstimatedCoinNZD { get; set; }

		public int WithdrawPercent
		{
			get
			{
				if (WithdrawLimit > 0 && WithdrawTotal > 0)
				{
					return (int)Math.Min(100, Math.Abs((WithdrawTotal / WithdrawLimit) * 100));
				}
				return 0;
			}
		}
	}
}
