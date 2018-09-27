using Cryptopia.Common.TwoFactor;
using Cryptopia.Entity;
using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Admin.Helpers
{
	public static class TwoFactorHelpers
	{
		public static T GetTwoFactorModel<T>(this ApplicationUser user, TwoFactorComponent component)
			where T : IVerifyTwoFactorModel, new()
		{
			if (user.TwoFactor == null)
			{
				return new T { Type = TwoFactorType.None };
			}

			var result = user.TwoFactor.FirstOrDefault(x => x.Component == component);
			if (result == null)
			{
				return new T { Type = TwoFactorType.None };
			}

			var model = new T();
			model.Type = result.Type;

			if (model.Type == TwoFactorType.Question)
			{
				model.Question1 = result.Data;
				model.Question2 = result.Data3;
			}

			return model;
		}
	}
}