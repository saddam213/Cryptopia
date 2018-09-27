namespace Cryptopia.Admin.Core.UserVerification
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using System.Threading.Tasks;
	using Cryptopia.Admin.Common.UserVerification;
	using Cryptopia.Entity;
	using Cryptopia.Enums;
	using Cryptopia.Infrastructure.Common.DataContext;
	using Cryptopia.Infrastructure.Common.Email;
	using Cryptopia.Infrastructure.Common.Results;
	using Cryptopia.Common.User;

	public class UserVerificationWriter : IUserVerificationWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }
		public IEmailService EmailService { get; set; }
		public IUserSyncService UserSyncService { get; set; }


		public async Task<IWriterResult> RejectUser(string currentAdminId, int verificationId, string reason)
		{
			string userName;
			EmailMessageModel emailModel;

			using (var context = DataContextFactory.CreateContext())
			{
				var userVerification = await context.UserVerification
						.Include(uv => uv.User)
						.Where(uv => uv.Id == verificationId)
						.FirstOrDefaultNoLockAsync();

				if (userVerification == null)
				{
					return new WriterResult(false, "User Verification Entity not found");
				}

				var user = userVerification.User;

				if (userVerification.User.VerificationLevel != VerificationLevel.Level2Pending)
				{
					return new WriterResult(false, "User Verification level not at Level 2 Pending");
				}


				var rejectedUserEntity = UserVerificationReject.CreateFrom(userVerification);
				rejectedUserEntity.RejectReason = reason;
				context.UserVerificationReject.Add(rejectedUserEntity);

				user.VerificationLevel = VerificationLevel.Level1;

				context.UserVerification.Remove(userVerification);
                context.LogActivity(currentAdminId, $"Rejected User Verification for {userVerification.FirstName} {userVerification.LastName}. Reason: {reason}");

				await context.SaveChangesAsync();

				var emailParameters = new List<object> { user.UserName, reason };
				emailModel = new EmailMessageModel
				{
					BodyParameters = emailParameters.ToArray(),
					Destination = user.Email,
					EmailType = EmailTemplateType.UserAccountRejected
				};

				userName = user.UserName;
				
			}

			await EmailService.SendEmail(emailModel);

			return new WriterResult(true, $"User {userName} Rejected - Email sent");
		}

		public async Task<IWriterResult> AcceptUser(int verificationId, string currentAdminId)
		{
			EmailMessageModel emailModel;
			string userName;
			string userId;

			using (var context = DataContextFactory.CreateContext())
			{
				var userVerification = await context.UserVerification
					.Include(uv => uv.User)
					.Where(uv => uv.Id == verificationId)
					.FirstOrDefaultNoLockAsync();

				if (userVerification == null)
				{
					return new WriterResult(false, "User Verification Entity not found");
				}

				var user = userVerification.User;

				if (userVerification.User.VerificationLevel != VerificationLevel.Level2Pending)
				{
					return new WriterResult(false, "User Verification level not at Level 2 Pending");
				}

				userVerification.ApprovedBy = currentAdminId;
				userVerification.Approved = DateTime.UtcNow;
				user.VerificationLevel = VerificationLevel.Level2;

				var emailParameters = new List<object> {user.UserName};

				emailModel = new EmailMessageModel
				{
					BodyParameters = emailParameters.ToArray(),
					Destination = user.Email,
					EmailType = EmailTemplateType.UserAccountVerified
				};

				userId = user.Id;
				userName = user.UserName;
                context.LogActivity(currentAdminId, $"Accepted User Verification for {userVerification.FirstName} {userVerification.LastName}.");
				await context.SaveChangesAsync();
			}

			await UserSyncService.SyncUser(userId);
			await EmailService.SendEmail(emailModel);

			return new WriterResult(true, $"User {userName} Verified - Email sent.");
		}
	}
}
