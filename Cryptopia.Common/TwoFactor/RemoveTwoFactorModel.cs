﻿using Cryptopia.Enums;

namespace Cryptopia.Common.TwoFactor
{
	public class RemoveTwoFactorModel
	{
		public TwoFactorType Type { get; set; }
		public TwoFactorComponent ComponentType { get; set; }
		public string Data { get; set; }
		public string Data2 { get; set; }
		public string Question1 { get; set; }
		public string Question2 { get; set; }
	}
}