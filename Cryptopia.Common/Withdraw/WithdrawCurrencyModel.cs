using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cryptopia.Common.TwoFactor;
using Cryptopia.Common.Validation;
using Cryptopia.Enums;
using Cryptopia.Common.Address;
using Cryptopia.Common.Currency;
using System;

namespace Cryptopia.Common.Withdraw
{
	public class WithdrawCurrencyModel : IVerifyTwoFactorModel
	{
		public string Name { get; set; }
		public string Symbol { get; set; }
		public decimal Balance { get; set; }
		public decimal Fee { get; set; }
		public WithdrawFeeType WithdrawFeeType { get; set; }
		public decimal MinWithdraw { get; set; }
		public decimal MaxWithdraw { get; set; }

		[RequiredBase]
		public int CurrencyId { get; set; }

		[Display(Name = nameof(Resources.User.withdrawAmountLabel), ResourceType = typeof(Resources.User))]
		public decimal Amount { get; set; }

		[RequiredBase]
		[StringLengthBase(1)]
		public string AddressData { get; set; }
		public string AddressData2 { get; set; }
		public string SelectedAddress { get; set; }
		public bool AddressBookOnly { get; set; }
		public List<AddressBookModel> AddressBook { get; set; }
		public CurrencyType CurrencyType { get; set; }
		public AddressType AddressType { get; set; }

		public string DisplayName
		{
			get { return $"{Name}({Symbol})"; }
		}
		public string ReturnUrl { get; set; }

		public string Message { get; set; }
		public string MessageType { get; set; }
		public string Instructions { get; set; }
		public decimal WithdrawLimit { get; set; }
		public decimal WithdrawTotal { get; set; }
		public bool HasWithdrawLimit { get; set; }

		public int WithdrawPercent
		{
			get
			{
				if(WithdrawLimit > 0 && WithdrawTotal > 0)
				{
					return (int)Math.Min(100, Math.Abs((WithdrawTotal / WithdrawLimit) * 100));
				}
				return 0;
			}
		}

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

		public decimal EstimatedCoinNZD { get; set; }
		public int Decimals { get; set; }

		#endregion
	}





	


	public class TipbotModel : IVerifyTwoFactorModel
	{
		public TipbotModel()
		{
			ActiveMin = 10;
		}

		public decimal Amount { get; set; }

		[Display(Name = nameof(Resources.User.tipCurrencyChatHandleLabel), ResourceType = typeof(Resources.User))]
		public string ChatHandle { get; set; }

		[Display(Name = nameof(Resources.User.tipCurrencyChatHandleListLabel), ResourceType = typeof(Resources.User))]
		public string ChatHandles { get; set; }

		[Display(Name = nameof(Resources.User.tipCurrencyChatUsersLabel), ResourceType = typeof(Resources.User))]
		public int ActiveMin { get; set; }

		[Display(Name = nameof(Resources.User.tipCurrencyLabel), ResourceType = typeof(Resources.User))]
		public int CurrencyId { get; set; }

		[Display(Name = nameof(Resources.User.tipCurrencyTypeLabel), ResourceType = typeof(Resources.User))]
		public TipbotPayoutType TipPayoutType { get; set; }

		[MaxLength(200)]
		[Display(Name = nameof(Resources.User.tipCurrnecyReasonLabel), ResourceType = typeof(Resources.User))]
		public string Reason { get; set; }

		public List<CurrencyModel> Currencies { get; set; }

		#region Twofactor

		public TwoFactorType Type { get; set; }

		[RequiredIf("Type", TwoFactorType.EmailCode, ErrorMessage = "Code is required.")]
		public string EmailCode { get; set; }

		[RequiredIf("Type", TwoFactorType.GoogleCode, ErrorMessage = "Code is required.")]
		public string GoogleCode { get; set; }

		[RequiredIf("Type", TwoFactorType.CryptopiaCode, ErrorMessage = "Code is required.")]
		public string CryptopiaCode { get; set; }

		[RequiredIf("Type", TwoFactorType.PinCode, ErrorMessage = "Pin is required.")]
		public string Pin { get; set; }

		[RequiredIf("Type", TwoFactorType.Password, ErrorMessage = "Password is required.")]
		public string Password { get; set; }

		[Display(Name = "Security Question 1")]
		public string Question1 { get; set; }

		[Display(Name = "Security Question 2")]
		public string Question2 { get; set; }

		[RequiredIf("Type", TwoFactorType.Question, ErrorMessage = "Answer 1 is required.")]
		public string Answer1 { get; set; }

		[RequiredIf("Type", TwoFactorType.Question, ErrorMessage = "Answer 2 is required.")]
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
						return "Please enter email code.";
					case TwoFactorType.GoogleCode:
						return "Please enter Google Authenticator code.";
					case TwoFactorType.CryptopiaCode:
						return "Please enter your Cryptopia Authenticator code.";
					case TwoFactorType.PinCode:
						return "Please enter pin code.";
					case TwoFactorType.Password:
						return "Please enter password.";
					case TwoFactorType.Question:
						return "Please answer the security questions.";
					default:
						break;
				}
				return string.Empty;
			}
		}

		#endregion
	}

	public enum TipbotPayoutType
	{
		SingleUser,
		MultipleUser,
		ActiveChatUsers
	}


	public class WithdrawCryptoNoteModel : IVerifyTwoFactorModel
	{
		public string Symbol { get; set; }
		public decimal Balance { get; set; }
		public decimal Fee { get; set; }
		public decimal MinWithdraw { get; set; }
		public decimal MaxWithdraw { get; set; }
		public bool DisableEmailConfirmation { get; set; }
		public CurrencyType CurrencyType { get; set; }

		[Required]
		public int CurrencyId { get; set; }

		[Required]
		public decimal Amount { get; set; }

		public string PaymentId { get; set; }

		[Required]
		public string Address { get; set; }

		#region Twofactor

		public TwoFactorType Type { get; set; }

		[RequiredIf("Type", TwoFactorType.EmailCode, ErrorMessage = "Code is required.")]
		public string EmailCode { get; set; }

		[RequiredIf("Type", TwoFactorType.GoogleCode, ErrorMessage = "Code is required.")]
		public string GoogleCode { get; set; }

		[RequiredIf("Type", TwoFactorType.CryptopiaCode, ErrorMessage = "Code is required.")]
		public string CryptopiaCode { get; set; }

		[RequiredIf("Type", TwoFactorType.PinCode, ErrorMessage = "Pin is required.")]
		public string Pin { get; set; }

		[RequiredIf("Type", TwoFactorType.Password, ErrorMessage = "Password is required.")]
		public string Password { get; set; }

		[Display(Name = "Security Question 1")]
		public string Question1 { get; set; }

		[Display(Name = "Security Question 2")]
		public string Question2 { get; set; }

		[RequiredIf("Type", TwoFactorType.Question, ErrorMessage = "Answer 1 is required.")]
		public string Answer1 { get; set; }

		[RequiredIf("Type", TwoFactorType.Question, ErrorMessage = "Answer 2 is required.")]
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
						return "Please enter email code.";
					case TwoFactorType.GoogleCode:
						return "Please enter Google Authenticator code.";
					case TwoFactorType.CryptopiaCode:
						return "Please enter your Cryptopia Authenticator code.";
					case TwoFactorType.PinCode:
						return "Please enter pin code.";
					case TwoFactorType.Password:
						return "Please enter password.";
					case TwoFactorType.Question:
						return "Please answer the security questions.";
					default:
						break;
				}
				return string.Empty;
			}
		}

		#endregion
	}
}