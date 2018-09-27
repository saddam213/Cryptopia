using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Admin.Common.AdminUser;
using Cryptopia.Base;
using Cryptopia.Common.User;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.Email;
using Cryptopia.Infrastructure.Common.Results;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cryptopia.Admin.Core.User
{
	public class AdminUserWriter : IAdminUserWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }
		public IUserSyncService UserSyncService { get; set; }
		public IEmailService EmailService { get; set; }


		public async Task<IWriterResult> ChangeEmail(string userId, AdminChangeEmailModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.UserName == model.UserName);
				if (user == null)
					return new WriterResult(false, $"User '{model.UserName}' not found");

				var existing = await context.Users.FirstOrDefaultNoLockAsync(x => x.Email == model.NewEmailAddress);
				if (existing != null)
					return new WriterResult(false, $"Email '{model.NewEmailAddress}' is already in use.");

				var approval = await context.ApprovalQueue.FirstOrDefaultNoLockAsync(x => x.DataUserId == user.Id && x.Type == Enums.ApprovalQueueType.ChangeEmail && x.Status == Enums.ApprovalQueueStatus.Pending);
				if (approval != null)
					return new WriterResult(false, "There is already an awaiting approval request for this user.");

				approval = new Entity.ApprovalQueue
				{
					DataUserId = user.Id,
					RequestUserId = userId,
					Type = Enums.ApprovalQueueType.ChangeEmail,
					Status = Enums.ApprovalQueueStatus.Pending,
					Created = DateTime.UtcNow,
					Data = JsonConvert.SerializeObject(model)
				};
				context.ApprovalQueue.Add(approval);
				await context.SaveChangesAsync();
				return new WriterResult(true, "Email change request added to admin approval queue.");
			}
		}

		public async Task<IWriterResult> ApproveChangeEmail(string userId, AdminChangeEmailApproveModel model)
		{
			var syncUserId = string.Empty;
			using (var context = DataContextFactory.CreateContext())
			{
				if (model.Status == ApprovalQueueStatus.Pending)
					return new WriterResult(false, $"Unable to set item to {model.Status}.");

				var approval = await context.ApprovalQueue.FirstOrDefaultNoLockAsync(x => x.Id == model.ApprovalId && x.Type == ApprovalQueueType.ChangeEmail);
				if (approval == null)
					return new WriterResult(false, "Not Found.");

				if (approval.Status != Enums.ApprovalQueueStatus.Pending)
					return new WriterResult(false, $"Unable to approve {approval.Type} status is {approval.Status}.");

				if (approval.RequestUserId == userId)
					return new WriterResult(false, $"Another admin must approve this {approval.Type}.");

				var existing = await context.Users.FirstOrDefaultNoLockAsync(x => x.Email == approval.Data);
				if (existing != null && model.Status == ApprovalQueueStatus.Approved)
					return new WriterResult(false, $"Email '{approval.Data}' is already in use.");

				approval.Status = model.Status;
				approval.Message = model.Message;
				approval.ApproveUserId = userId;
				approval.Approved = DateTime.UtcNow;
				if (model.Status == ApprovalQueueStatus.Approved)
				{
					syncUserId = approval.DataUserId;
                    approval.DataUser.Email = JsonConvert.DeserializeObject<AdminChangeEmailModel>(approval.Data).NewEmailAddress;
                }
				await context.SaveChangesAsync();
			}
			await UserSyncService.SyncUser(syncUserId);
			return new WriterResult(true, model.Status == ApprovalQueueStatus.Approved ? "Action approved, users email address has now been updated." : "Action Rejected, email change rejected.");
		}

		public async Task<IWriterResult> ResetTwoFactor(string userId, AdminResetTwoFactorModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == model.UserName);
				if (user == null)
					return new WriterResult(false, $"User '{model.UserName}' not found");

				var approval = await context.ApprovalQueue.FirstOrDefaultNoLockAsync(x => x.DataUserId == user.Id && x.Type == Enums.ApprovalQueueType.ResetTwoFactor && x.Status == Enums.ApprovalQueueStatus.Pending);
				if (approval != null)
				{
					return new WriterResult(false, "Already awaiting approval.");
				}

				approval = new Entity.ApprovalQueue
				{
					DataUserId = user.Id,
					RequestUserId = userId,
					Type = Enums.ApprovalQueueType.ResetTwoFactor,
					Status = Enums.ApprovalQueueStatus.Pending,
					Created = DateTime.UtcNow,
					Data = model.Type
				};
				context.ApprovalQueue.Add(approval);
				await context.SaveChangesAsync();
				return new WriterResult(true, $"Reset Two Factor for user {user.UserName} added to Approval Queue");
			}
		}

		public async Task<IWriterResult> ResetAllTwoFactor(string userId, AdminResetTwoFactorModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.UserName == model.UserName);
				if (user == null)
					return new WriterResult(false, $"User '{model.UserName}' not found");

				var approval = await context.ApprovalQueue.FirstOrDefaultNoLockAsync(x => x.DataUserId == user.Id && x.Type == Enums.ApprovalQueueType.ResetAllTwoFactor && x.Status == Enums.ApprovalQueueStatus.Pending);
				if (approval != null)
				{
					return new WriterResult(false, "Already awaiting approval.");
				}

				approval = new Entity.ApprovalQueue
				{
					DataUserId = user.Id,
					RequestUserId = userId,
					Type = Enums.ApprovalQueueType.ResetAllTwoFactor,
					Status = Enums.ApprovalQueueStatus.Pending,
					Created = DateTime.UtcNow,
					Data = model.Type
				};

				context.ApprovalQueue.Add(approval);
				await context.SaveChangesAsync();
				return new WriterResult(true, $"Reset Two Factor for user {user.UserName} added to Approval Queue");
			}
		}

		public async Task<IWriterResult> ApproveResetAllTwoFactor(string userId, AdminResetTwoFactorApproveModel model)
		{
			EmailMessageModel emailModel = new EmailMessageModel();
			using (var context = DataContextFactory.CreateContext())
			{
				var approval = await context.ApprovalQueue.FirstOrDefaultNoLockAsync(x => x.Id == model.ApprovalId && x.Type == ApprovalQueueType.ResetAllTwoFactor);
				if (approval == null)
					return new WriterResult(false, "Not Found.");

				if (approval.Status != Enums.ApprovalQueueStatus.Pending)
					return new WriterResult(false, $"Unable to approve {approval.Type} status is {approval.Status}.");

				if (approval.RequestUserId == userId)
					return new WriterResult(false, $"Another admin must approve this {approval.Type}.");

				approval.Status = model.Status;
				approval.Message = model.Message;

				if (model.Status != ApprovalQueueStatus.Pending)
				{
					approval.ApproveUserId = userId;
					approval.Approved = DateTime.UtcNow;
				}

				if (model.Status == ApprovalQueueStatus.Approved)
				{
					int randomPin = ObjectExtensions.GetRandomNumber();
					var twoFactorItems = await context.UserTwoFactor.Where(x => x.UserId == approval.DataUserId && x.Type != TwoFactorType.None).ToListNoLockAsync();
					foreach (var twoFactorItem in twoFactorItems)
					{
						if (twoFactorItem.Type == TwoFactorType.None) continue;

						twoFactorItem.Type = Enums.TwoFactorType.PinCode;
						twoFactorItem.Data = randomPin.ToString();
						twoFactorItem.Data2 = string.Empty;
						twoFactorItem.Data3 = string.Empty;
						twoFactorItem.Data4 = string.Empty;
						twoFactorItem.Data5 = string.Empty;
					}

					var user = await context.Users.Where(x => x.Id == approval.DataUserId).FirstOrDefaultNoLockAsync();
					var emailParameters = new List<object> { user.UserName, randomPin };
					emailModel = new EmailMessageModel
					{
						BodyParameters = emailParameters.ToArray(),
						Destination = user.Email,
						EmailType = EmailTemplateType.TwoFactorReset
					};

				}
				await context.SaveChangesAsync();
			}

			if (model.Status == ApprovalQueueStatus.Approved)
			{
				await EmailService.SendEmail(emailModel);
				return new WriterResult(true, $"Action approved, Two factor reset email email has been sent to {emailModel.Destination}");
			}

			return new WriterResult(true, "Action Rejected, two factor reset rejected.");
		}


		public async Task<IWriterResult> UpdateUser(AdminUserUpdateModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == model.Id);
				if (user == null)
					return new WriterResult(false, $"User '{model.UserName}' not found");

				user.UserName = model.UserName;
				user.EmailConfirmed = model.EmailConfirmed;
				user.RoleCss = model.RoleCss;
				user.ShareCount = model.ShareCount;
				await context.SaveChangesAsync();

				model.UserId = user.Id;
				return new WriterResult(true, "Successfully updated user.");
			}
		}

		public async Task<IWriterResult> UpdateSettings(string adminUserId, AdminUserSettingsUpdateModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var settings = await context.UserSettings.FirstOrDefaultNoLockAsync(x => x.Id == model.Id);
				if (settings == null)
					return new WriterResult(false, $"User '{model.UserName}' not found");

				settings.HideZeroBalance = model.HideZeroBalance;
				settings.ShowFavoriteBalance = model.ShowFavoriteBalance;
				settings.Theme = model.Theme;
				await context.SaveChangesWithAuditAsync(adminUserId);

				return new WriterResult(true, "Successfully updated user settings.");
			}
		}

		public async Task<IWriterResult> UpdateApi(string adminUserId, AdminUserApiUpdateModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == model.Id);
				if (user == null)
					return new WriterResult(false, $"User '{model.UserName}' not found");

				user.IsApiEnabled = model.IsApiEnabled;
				user.IsApiUnsafeWithdrawEnabled = model.IsApiUnsafeWithdrawEnabled;
				user.IsApiWithdrawEnabled = model.IsApiWithdrawEnabled;

                await context.SaveChangesWithAuditAsync(adminUserId);

				return new WriterResult(true, "Successfully updated user API settings.");
			}
		}

		public async Task<IWriterResult> UpdateProfile(AdminUserProfileUpdateModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var profile = await context.UserProfiles.FirstOrDefaultNoLockAsync(x => x.Id == model.Id);
				if (profile == null)
					return new WriterResult(false, $"User '{model.UserName}' not found");

				profile.AboutMe = model.AboutMe;
				profile.Address = model.Address;
				profile.Birthday = model.Birthday;
				profile.City = model.City;
				profile.ContactEmail = model.ContactEmail;
				profile.Country = model.Country;
				profile.Education = model.Education;
				profile.Facebook = model.Facebook;
				profile.FirstName = model.FirstName;
				profile.Gender = model.Gender;
				profile.Hobbies = model.Hobbies;
				profile.LastName = model.LastName;
				profile.LinkedIn = model.LinkedIn;
				//profile.Location = model.Location;
				profile.Occupation = model.Occupation;
				profile.Postcode = model.Postcode;
				profile.State = model.State;
				profile.Twitter = model.Twitter;
				profile.Website = model.Website;
				//profile.ShowAddress = model.ShowAddress;
				//profile.ShowDetails = model.ShowDetails;
				await context.SaveChangesAsync();

				return new WriterResult(true, "Successfully updated user profile.");
			}
		}

		public async Task<IWriterResult> ActivateUser(string adminUserId, AdminActivateUserModel model)
		{
			string userId = "";
			EmailMessageModel emailModel = new EmailMessageModel();
			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.UserName == model.UserName);
				if (user == null)
					return new WriterResult(false, $"User '{model.UserName}' not found");

				if(user.EmailConfirmed)
					return new WriterResult(false, $"User '{model.UserName}' already activated");

				userId = user.Id;
				user.EmailConfirmed = true;
                context.LogActivity(adminUserId, $"Activating user {user.UserName}");
				await context.SaveChangesAsync();

				var emailParameters = new List<object> { model.UserName };
				emailModel = new EmailMessageModel
				{
					BodyParameters = emailParameters.ToArray(),
					Destination = user.Email,
					EmailType = EmailTemplateType.AccountActivated
				};
			}
			await UserSyncService.SyncUser(userId);
			await EmailService.SendEmail(emailModel);
		
			return new WriterResult(true, $"Successfully activated {model.UserName} - Email sent.");
		}

		public async Task<IWriterResult> LockUser(string adminUserId, string userName)
		{
			var userId = "";

			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(u => u.UserName == userName).ConfigureAwait(false);
				if (user == null)
				{
					return new WriterResult(false, "User Not Found");
				}
				userId = user.Id;
				user.LockoutEndDateUtc = DateTime.UtcNow.AddYears(1);
				user.SecurityStamp = Guid.NewGuid().ToString();

                context.LogActivity(adminUserId, $"Locked User {userName}");
				await context.SaveChangesAsync().ConfigureAwait(false);
			}

			await UserSyncService.SyncUser(userId);
			return new WriterResult(true, $"User '{userName}' Successfully Locked.");
		}

		public async Task<IWriterResult> UnlockUser(string adminUserId, string userName)
		{
			var userId = "";

			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(u => u.UserName == userName).ConfigureAwait(false);
				if (user == null)
				{
					return new WriterResult(false, "User Not Found");
				}
				userId = user.Id;
				user.LockoutEndDateUtc = null;
				user.SecurityStamp = Guid.NewGuid().ToString();

                context.LogActivity(adminUserId, $"Unlocking user {userName}");
                await context.SaveChangesAsync().ConfigureAwait(false);
			}

			await UserSyncService.SyncUser(userId);
			return new WriterResult(true, $"User '{userName}' Successfully Unlocked.");
		}

		public async Task<IWriterResult> DisableUser(string adminUserId, string userName)
		{
			var userId = "";

			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(u => u.UserName == userName).ConfigureAwait(false);
				if (user == null)
				{
					return new WriterResult(false, "User Not Found");
				}

				userId = user.Id;
				user.IsDisabled = true;
				user.SecurityStamp = Guid.NewGuid().ToString();

                context.LogActivity(adminUserId, $"Disabling User {userName}");
                await context.SaveChangesAsync().ConfigureAwait(false);
			}

			await UserSyncService.SyncUser(userId);
			return new WriterResult(true, $"User '{userName}' Successfully Disabled.");
		}

	}
}
