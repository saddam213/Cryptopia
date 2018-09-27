using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Enums;
using System.Data.Entity;
using Cryptopia.Infrastructure.Common.Results;
using Cryptopia.Entity.Auditing;

namespace Cryptopia.Core.User
{
	public class UserSettingsWriter : IUserSettingsWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }
		public IUserSyncService UserSyncService { get; set; }

		public async Task<WriterResult> UpdateSettings(string userId, UserSettingsModel model)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var user = await context.Users
						.Include(u => u.Settings)
						.FirstOrDefaultAsync(x => x.Id == userId).ConfigureAwait(false);
					if (user == null)
						return new WriterResult(false, "User not found.");

					user.ChatDisableEmoticons = model.DisableChatEmoticons;
					user.DisableLogonEmail = model.DisableLogonEmail;
					user.DisableTips = model.DisableTips;
					user.DisableTipNotify = model.DisableTipNotify;
					user.DisablePoolNotify = model.DisablePoolNotify;
					user.DisableExchangeNotify = model.DisableExchangeNotify;
					user.DisableFaucetNotify = model.DisableFaucetNotify;
					user.DisableMarketplaceNotify = model.DisableMarketplaceNotify;
					user.DisableRewards = model.DisableRewards;
					user.DisableKarmaNotify = model.DisableKarmaNotify;
					user.Settings.Theme = model.Theme;

                    await context.SaveChangesWithAuditAsync().ConfigureAwait(false);
				}

				await UserSyncService.SyncUser(userId).ConfigureAwait(false);
				return new WriterResult(true, "Successfully updated settings.");
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}

		public async Task<WriterResult<bool>> UpdateIgnoreList(string userId, UserIgnoreListType type, string username)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId).ConfigureAwait(false);
					if (user == null)
						return new WriterResult<bool>(false, "User not found.");

					var userToIgnore = await context.Users.FirstOrDefaultAsync(x => x.UserName == username).ConfigureAwait(false);
					if (userToIgnore == null)
						return new WriterResult<bool>(false, "User '{0}' not found.", username);

					bool wasRemove = false;
					string result = string.Empty;
					switch (type)
					{
						case UserIgnoreListType.Tip:
							result = AddOrRemoveFromList(user.ChatTipIgnoreList, username);
							wasRemove = result.Length < user.ChatTipIgnoreList?.Length;
							user.ChatTipIgnoreList = result;
							break;
						case UserIgnoreListType.Chat:
							result = AddOrRemoveFromList(user.ChatIgnoreList, username);
							wasRemove = result.Length < user.ChatIgnoreList?.Length;
							user.ChatIgnoreList = result;
							break;
						case UserIgnoreListType.Message:
							break;
						case UserIgnoreListType.Forum:
							break;
						default:
							break;
					}

					await context.SaveChangesAsync().ConfigureAwait(false);
					return new WriterResult<bool>(true, wasRemove, "{0} successfully {1} your {2} ignore list", username,
						wasRemove ? "removed from" : "added to", type.ToString().ToLower());
				}
			}
			catch (Exception)
			{
				return new WriterResult<bool>(false);
			}
		}

		public async Task<WriterResult> UpdateChartSettings(string userId, string settings)
		{
			if (string.IsNullOrEmpty(settings) || !settings.Contains(",") || settings.Split(',').Count() != 12)
				return new WriterResult(false, "Failed to update chart settings, Invalid settings.");

			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId).ConfigureAwait(false);
				if (user == null)
					return new WriterResult(false, "User not found.");

				user.ChartSettings = settings;
				await context.SaveChangesAsync().ConfigureAwait(false);
				return new WriterResult(true, "Successfully updated chart settings.");
			}
		}

		public async Task<WriterResult> UpdateTheme(string userId, SiteTheme theme)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var user = context.Users
						.Include(u => u.Settings)
						.FirstOrDefault(x => x.Id == userId);
					if (user == null)
						return new WriterResult(false, "User not found.");

					user.Settings.Theme = theme;
					await context.SaveChangesAsync().ConfigureAwait(false);
				}

				return new WriterResult(true, "Successfully updated theme.");
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}

		public async Task<IWriterResult> UpdateBalanceFavoriteOnly(string userId, bool favoritesOnly)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var user = context.Users
						.Include(u => u.Settings)
						.FirstOrDefault(x => x.Id == userId);
					if (user == null)
						return new WriterResult(false, "User not found.");

					user.Settings.ShowFavoriteBalance = favoritesOnly;
					await context.SaveChangesAsync().ConfigureAwait(false);
				}

				return new WriterResult(true, "Successfully updated balance settings.");
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}

		public async Task<IWriterResult> UpdateBalanceHideZero(string userId, bool hideZero)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var user = context.Users
						.Include(u => u.Settings)
						.FirstOrDefault(x => x.Id == userId);
					if (user == null)
						return new WriterResult(false, "User not found.");

					user.Settings.HideZeroBalance = hideZero;
					await context.SaveChangesAsync().ConfigureAwait(false);
				}
				return new WriterResult(true, "Successfully updated balance settings.");
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}

		private string AddOrRemoveFromList(string input, string username)
		{
			var list = string.IsNullOrEmpty(input)
				? new List<string>()
				: input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
			if (list.Contains(username))
			{
				list.Remove(username);
			}
			else
			{
				list.Add(username);
			}
			return string.Join(",", list);
		}
	}
}