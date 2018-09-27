using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.DataContext;
using System.Data.Entity;

namespace Cryptopia.Core.User
{
	public class UserSettingsReader : IUserSettingsReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<UserSettingsModel> GetSettings(string userId)
		{
			try
			{
				using (var context = DataContextFactory.CreateReadOnlyContext())
				{
					var user = await context.Users
						.AsNoTracking()
						.Include(u => u.Settings)
						.Where(x => x.Id == userId)
						.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (user == null)
						return null;

					var model = new UserSettingsModel
					{
						DisableChatEmoticons = user.ChatDisableEmoticons,
						DisableExchangeNotify = user.DisableExchangeNotify,
						DisableFaucetNotify = user.DisableFaucetNotify,
						DisableKarmaNotify = user.DisableKarmaNotify,
						DisableLogonEmail = user.DisableLogonEmail,
						DisableMarketplaceNotify = user.DisableMarketplaceNotify,
						DisablePoolNotify = user.DisablePoolNotify,
						DisableRewards = user.DisableRewards,
						DisableTipNotify = user.DisableTipNotify,
						DisableTips = user.DisableTips,
						Theme = user.Settings.Theme,
						IgnoreChatList = !string.IsNullOrEmpty(user.ChatIgnoreList)
							? new List<string>(user.ChatIgnoreList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()))
							: new List<string>(),
						IgnoreTipList = !string.IsNullOrEmpty(user.ChatTipIgnoreList)
							? new List<string>(user.ChatTipIgnoreList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()))
							: new List<string>()
					};
					return model;
				}
			}
			catch (Exception)
			{
				return new UserSettingsModel();
			}
		}
	}
}