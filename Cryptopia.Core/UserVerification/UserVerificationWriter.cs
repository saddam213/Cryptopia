using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.UserVerification;
using Cryptopia.Enums;
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.Verification
{
	public class UserVerificationWriter : IUserVerificationWriter
	{
		public IUserSyncService UserSyncService { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<IWriterResult> CreateVerification(string userId, UserVerificationModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == userId);
				if (user.VerificationLevel != VerificationLevel.Level1 && user.VerificationLevel != VerificationLevel.Legacy)
					return new WriterResult(false, $"Verification has already been submitted.");

				var verification = await context.UserVerification.FirstOrDefaultNoLockAsync(x => x.UserId == userId);
				if (verification != null)
					return new WriterResult(false, $"Verification has already been submitted.");

				verification = new Entity.UserVerification
				{
					UserId = userId,
					FirstName = model.FirstName,
					LastName = model.LastName,
					Birthday = model.Birthday,
					Gender = model.Gender,
					Address = model.Address,
					City = model.City,
					State = model.State,
					Postcode = model.Postcode,
					Country = model.Country,
					Identification1 = model.Identification1,
					Identification2 = model.Identification2,
					Timestamp = DateTime.UtcNow
				};
				context.UserVerification.Add(verification);
				user.VerificationLevel = VerificationLevel.Level2Pending;
				await context.SaveChangesAsync();
			}
			await UserSyncService.SyncUser(userId);
			return new WriterResult(true, $"Successfully submitted verification information.");
		}

		public async Task<IWriterResult> AdminApproveVerification(string userId, ApproveVerificationModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var approvalUser = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == userId);
				var verification = await context.UserVerification
					.Include(x => x.User)
					.FirstOrDefaultNoLockAsync(x => x.Id == model.VerificationId);
				if (verification == null)
					return new WriterResult(false, $"Verification not found.");

				verification.Approved = DateTime.UtcNow;
				verification.ApprovedBy = approvalUser.Id;
				verification.User.VerificationLevel = model.VerificationLevel;
				await context.SaveChangesAsync();
			}
			await UserSyncService.SyncUser(userId);
			return new WriterResult(true, $"Successfully approved {model.VerificationLevel} verification.");
		}
	}
}