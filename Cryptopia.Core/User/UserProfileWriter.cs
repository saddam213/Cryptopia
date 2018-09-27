using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.User;
using System;
using System.Threading.Tasks;
using System.Data.Entity;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.User
{
	public class UserProfileWriter : IUserProfileWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }
		public IUserSyncService UserSyncService { get; set; }

		public async Task<WriterResult> UpdateProfile(string userId, UserProfileModel model)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var user = await context.Users
						.Include(x => x.Profile)
						.Include(x => x.Settings)
						.FirstOrDefaultAsync(x => x.Id == userId).ConfigureAwait(false);
					if (user == null)
						return new WriterResult(false, "An unknown error occured");

					if (user.ChatHandle != model.ChatHandle && await context.Users.AnyAsync(x => x.Id != user.Id && (x.ChatHandle == model.ChatHandle || x.UserName == model.ChatHandle || x.MiningHandle == model.ChatHandle)).ConfigureAwait(false))
						return new WriterResult(false, "ChatHandle {0} has already been taken.", model.ChatHandle);

					if (user.MiningHandle != model.MiningHandle &&	await context.Users.AnyAsync(x => x.Id != user.Id && (x.ChatHandle == model.MiningHandle || x.UserName == model.MiningHandle || x.MiningHandle == model.MiningHandle)).ConfigureAwait(false))
						return new WriterResult(false, "MiningHandle {0} has already been taken.", model.MiningHandle);

					user.ChatHandle = model.ChatHandle;
					user.MiningHandle = model.MiningHandle;

					user.Profile.IsPublic = model.IsPublic;
					user.Profile.FirstName = model.FirstName;
					user.Profile.LastName = model.LastName;
					user.Profile.Address = model.Address;
					user.Profile.Postcode = model.Postcode;
					user.Profile.Country = model.Country;
					user.Profile.City = model.City;
					user.Profile.State = model.State;

					user.Profile.ContactEmail = model.ContactEmail;
					user.Profile.AboutMe = model.AboutMe;
					user.Profile.Gender = model.Gender;
					user.Profile.Birthday = model.Birthday;
					user.Profile.Occupation = model.Occupation;
					user.Profile.Hobbies = model.Hobbies;
					user.Profile.Education = model.Education;
					user.Profile.Website = model.Website;
					user.Profile.Facebook = model.Facebook;
					user.Profile.Twitter = model.Twitter;
					user.Profile.LinkedIn = model.LinkedIn;

					await context.SaveChangesWithAuditAsync().ConfigureAwait(false);
				}

				await UserSyncService.SyncUser(userId).ConfigureAwait(false);
				return new WriterResult(true, "Successfully updated account details.");
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}
	}
}