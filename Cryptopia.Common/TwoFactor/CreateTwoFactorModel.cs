using System.ComponentModel.DataAnnotations;
using Cryptopia.Enums;

namespace Cryptopia.Common.TwoFactor
{
	public class CreateTwoFactorModel
	{
		[Display(Name = nameof(Resources.Authorization.twoFactorTypeLabel), ResourceType = typeof(Resources.Authorization))]
		public TwoFactorType Type { get; set; }

		public TwoFactorComponent ComponentType { get; set; }

		[Display(Name = nameof(Resources.Authorization.twoFactorDataEmailLabel), ResourceType = typeof(Resources.Authorization))]
		public string DataEmail { get; set; }

		[RegularExpression("^[0-9]{4,8}$", ErrorMessageResourceName = nameof(Resources.Authorization.twoFactorDataPinPatternError), ErrorMessageResourceType = typeof(Resources.Authorization))]
		[Display(Name = nameof(Resources.Authorization.twoFactorDataPinLabel), ResourceType = typeof(Resources.Authorization))]
		public string DataPin { get; set; }

		public GoogleTwoFactorData GoogleData { get; set; }

		[Display(Name = nameof(Resources.Authorization.twoFactorDataQuestion1Label), ResourceType = typeof(Resources.Authorization))]
		public string DataQuestion1 { get; set; }

		[Display(Name = nameof(Resources.Authorization.twoFactorDataQuestion2Label), ResourceType = typeof(Resources.Authorization))]
		public string DataQuestion2 { get; set; }

		[Display(Name = nameof(Resources.Authorization.twoFactorDataAnswer1Label), ResourceType = typeof(Resources.Authorization))] 
		public string DataAnswer1 { get; set; }

		[Display(Name = nameof(Resources.Authorization.twoFactorDataAnswer2Label), ResourceType = typeof(Resources.Authorization))]
		public string DataAnswer2 { get; set; }

		public bool HasExistingGoogle { get; set; }
		public bool HasExistingCryptopia { get; set; }
		public string CryptopiaSerial { get; set; }
		public bool ApplyToAllEmpty { get; set; }
	}
}