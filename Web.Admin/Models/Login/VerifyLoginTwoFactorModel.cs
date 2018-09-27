using System.ComponentModel.DataAnnotations;
using Cryptopia.Common.TwoFactor;
using Cryptopia.Common.Validation;
using Cryptopia.Enums;

namespace Web.Admin.Models
{
	public class VerifyLoginTwoFactorModel : IVerifyTwoFactorModel
	{
		public TwoFactorType Type { get; set; }

		[RequiredIf("Type", TwoFactorType.EmailCode, ErrorMessage = "Code is required.")]
		public string EmailCode { get; set; }

		[RequiredIf("Type", TwoFactorType.GoogleCode, ErrorMessage = "Code is required.")]
		public string GoogleCode { get; set; }

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
	}
}