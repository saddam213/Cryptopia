using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.TwoFactor;
using Cryptopia.Entity;
using Cryptopia.Data.DataContext;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Helpers;
using Web.Site.Helpers;

namespace Web.Site.App_Start
{
	public class ApplicationUserManager : UserManager<ApplicationUser>
	{
		public ApplicationUserManager(IUserStore<ApplicationUser> store)
			: base(store)
		{
			this.UserValidator = new UserValidator<ApplicationUser>(this)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = true
			};
		}

		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
			IOwinContext context)
		{
			var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
			manager.UserValidator = new UserValidator<ApplicationUser>(manager)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = true
			};

			// Configure validation logic for passwords
			manager.PasswordValidator = new PasswordValidator
			{
				RequiredLength = 6,
				RequireNonLetterOrDigit = true,
				RequireDigit = true,
				RequireLowercase = true,
				RequireUppercase = true,
			};

			manager.UserLockoutEnabledByDefault = true;
			manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(15);
			manager.MaxFailedAccessAttemptsBeforeLockout = 3;


			manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>());


			var dataProtectionProvider = options.DataProtectionProvider;
			if (dataProtectionProvider != null)
			{
				manager.UserTokenProvider =
					new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ConfirmUser"));
			}
			return manager;
		}

		public async Task<string> GenerateUserTwoFactorCodeAsync(TwoFactorType codeType, string userid)
		{
			if (codeType == TwoFactorType.EmailCode)
			{
				return await GenerateTwoFactorTokenAsync(userid, codeType.ToString());
			}
			return string.Empty;
		}

		public async Task<UserTwoFactor> GetUserTwoFactorAsync(string userId, TwoFactorComponent component)
		{
			var user = await FindByIdAsync(userId);
			if (user == null)
				throw new UnauthorizedAccessException();

			return user.TwoFactor.FirstOrDefault(x => x.Component == component);
		}

		public async Task<bool> VerifyUserTwoFactorCodeAsync(TwoFactorComponent component, string userid, string data, string data2)
		{
			var user = await FindByIdAsync(userid);
			if (user == null)
				return false;

			var twofactorMethod = user.TwoFactor.FirstOrDefault(x => x.Component == component);
			if (twofactorMethod == null || twofactorMethod.Type == TwoFactorType.None)
				return true;

			if (twofactorMethod.Type == TwoFactorType.PinCode)
				return twofactorMethod.Data == data;

			if (twofactorMethod.Type == TwoFactorType.EmailCode)
				return await VerifyTwoFactorTokenAsync(userid, twofactorMethod.Type.ToString(), data);

			if (twofactorMethod.Type == TwoFactorType.GoogleCode)
				return GoogleAuthenticationHelper.VerifyGoogleTwoFactorCode(twofactorMethod.Data, data);

			if (twofactorMethod.Type == TwoFactorType.CryptopiaCode)
				return await CryptopiaAuthenticationHelper.VerifyTwoFactorCode(userid, data);

			if (twofactorMethod.Type == TwoFactorType.Password)
				return PasswordHasher.VerifyHashedPassword(user.PasswordHash, data) != PasswordVerificationResult.Failed;

			if (twofactorMethod.Type == TwoFactorType.Question)
				return data.Equals(twofactorMethod.Data2, StringComparison.OrdinalIgnoreCase) && data2.Equals(twofactorMethod.Data4, StringComparison.OrdinalIgnoreCase);

			return false;
		}

		public async Task<T> GetUserTwoFactorModelAsync<T>(string userId, TwoFactorComponent component) where T : IVerifyTwoFactorModel, new()
		{
			var user = await FindByIdAsync(userId);
			if (user?.TwoFactor == null)
				return new T { Type = TwoFactorType.None };

			var result = user.TwoFactor.FirstOrDefault(x => x.Component == component);
			if (result == null)
				return new T { Type = TwoFactorType.None };

			var model = new T
			{
				Type = result.Type
			};

			if (model.Type == TwoFactorType.Question)
			{
				model.Question1 = result.Data;
				model.Question2 = result.Data3;
			}

			return model;
		}
	}
}