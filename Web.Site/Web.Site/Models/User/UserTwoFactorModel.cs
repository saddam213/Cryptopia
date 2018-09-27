using System.ComponentModel.DataAnnotations;
using Cryptopia.Common.Validation;
using Cryptopia.Enums;

namespace Web.Site.Models
{
	public class UserTwoFactorModel
	{
		public TwoFactorComponent Component { get; set; }
		public TwoFactorType Type { get; set; }

		[Display(Name = "Email Address")]
		[EmailAddress]
		[RequiredIf("Type", TwoFactorType.EmailCode, ErrorMessage = "Email address is required.")]
		public string Email { get; set; }

		[Display(Name = "Pin Code")]
		[StringLength(8, ErrorMessage = "The pin must be between {2} and {1} digits.", MinimumLength = 4)]
		[RequiredIf("Type", TwoFactorType.PinCode, ErrorMessage = "Pin code is required.")]
		public string Pin { get; set; }

		[Display(Name = "Security Question 1")]
		[StringLength(50, ErrorMessage = "The question must be between {2} and {1} characters.", MinimumLength = 4)]
		[RequiredIf("Type", TwoFactorType.Question, ErrorMessage = "Question 1 is required.")]
		public string Question1 { get; set; }

		[Display(Name = "Security Question 2")]
		[StringLength(50, ErrorMessage = "The question must be between {2} and {1} characters.", MinimumLength = 4)]
		[RequiredIf("Type", TwoFactorType.Question, ErrorMessage = "Question 2 is required.")]
		public string Question2 { get; set; }

		[StringLength(50, ErrorMessage = "The answer must be between {2} and {1} characters.", MinimumLength = 4)]
		[RequiredIf("Type", TwoFactorType.Question, ErrorMessage = "Answer 1 is required.")]
		public string Answer1 { get; set; }

		[StringLength(50, ErrorMessage = "The answer must be between {2} and {1} characters.", MinimumLength = 4)]
		[RequiredIf("Type", TwoFactorType.Question, ErrorMessage = "Answer 2 is required.")]
		public string Answer2 { get; set; }

		[Display(Name = "Disable Confirmation Email")]
		public bool DisableEmailConfirmation { get; set; }

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

		public string ComponentSummary
		{
			get
			{
				switch (Component)
				{
					case TwoFactorComponent.Settings:
						return "Twofactor security used for unlocking sensitive user settings.";
					case TwoFactorComponent.Login:
						return "Twofactor security used when logging into Cryptopia.";
					case TwoFactorComponent.Lockout:
						return "Twofactor security used to reset an account lockout.";
					case TwoFactorComponent.Withdraw:
						return "Twofactor security used to submit a withdrawal.";
					case TwoFactorComponent.Transfer:
						return "Twofactor security used to submit a transfer.";
					case TwoFactorComponent.Tip:
						return "Twofactor security used to submit a tip.";
					default:
						break;
				}
				return string.Empty;
			}
		}

		public string Note
		{
			get
			{
				switch (Component)
				{
					case TwoFactorComponent.Settings:
						return "";
					case TwoFactorComponent.Login:
						return "";
					case TwoFactorComponent.Lockout:
						return "(Setting to 'None' will enforce the 24hr lockout)";
					case TwoFactorComponent.Withdraw:
						return "(Email confirmation will still be required to process the withdrawal)";
					case TwoFactorComponent.Transfer:
						return "";
					case TwoFactorComponent.Tip:
						return "";
					default:
						break;
				}
				return string.Empty;
			}
		}

		public int DisplayOrder
		{
			get
			{
				switch (Component)
				{
					case TwoFactorComponent.Settings:
						return 0;
					case TwoFactorComponent.Login:
						return 1;
					case TwoFactorComponent.Lockout:
						return 4;
					case TwoFactorComponent.Withdraw:
						return 2;
					case TwoFactorComponent.Transfer:
						return 3;
					case TwoFactorComponent.Tip:
						return 4;
					default:
						break;
				}
				return 1000;
			}

		}
	}
}