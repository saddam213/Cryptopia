using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.User;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using Cryptopia.Common.Referral;

namespace Cryptopia.Core.User
{
	public class UserProfileReader : IUserProfileReader
	{
		public IDataContextFactory DataContextFactory { get; set; }
		public IReferralReader ReferralReader { get; set; }

		public async Task<UserProfileModel> GetProfile(string userId)
		{
			try
			{
				using (var context = DataContextFactory.CreateReadOnlyContext())
				{
					var user = await context.Users
						.AsNoTracking()
						.Include(x => x.Profile)
						.Include(x => x.Settings)
						.Where(x => x.Id == userId)
						.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (user == null)
						return null;

					return new UserProfileModel
					{
						IsPublic = user.Profile.IsPublic,
						AboutMe = user.Profile.AboutMe,
						AccountEmail = user.Email,
						Birthday = user.Profile.Birthday,
						ChatHandle = user.ChatHandle,
						ContactEmail = user.Profile.ContactEmail,
						Education = user.Profile.Education,
						Facebook = user.Profile.Facebook,
						Gender = user.Profile.Gender,
						Hobbies = user.Profile.Hobbies,
						LinkedIn = user.Profile.LinkedIn,
						MiningHandle = user.MiningHandle,
						Occupation = user.Profile.Occupation,
						TrustRating = user.TrustRating,
						Twitter = user.Profile.Twitter,
						Website = user.Profile.Website,
						FirstName = user.Profile.FirstName,
						LastName = user.Profile.LastName,
						VerificationLevel = user.VerificationLevel,
						Address = user.Profile.Address,
						City = user.Profile.City,
						Country = user.Profile.Country,
						Postcode = user.Profile.Postcode,
						State = user.Profile.State,

						ReferralDetails = await ReferralReader.GetActiveReferral(user.Id).ConfigureAwait(false)
					};
			}
			}
			catch (Exception)
			{
				return new UserProfileModel();
	}
}

public async Task<UserProfileInfoModel> GetProfileInfo(string userId)
{
	try
	{
		using (var context = DataContextFactory.CreateReadOnlyContext())
		{
			var info = await context.Users
				.AsNoTracking()
				.Where(u => u.Id == userId)
				.Select(u => new UserProfileInfoModel
				{
					UserName = u.UserName,
					TrustRrating = u.TrustRating,
					KarmaPoints = u.KarmaTotal,
					UnreadMessages = u.Messages.Count(x => x.IsInbound && !x.IsRead),
					UnreadNotifications = u.Notifications.Count(x => !x.Acknowledged)
				}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);

			return info;
		}
	}
	catch (Exception)
	{
		return new UserProfileInfoModel();
	}
}
	}
}