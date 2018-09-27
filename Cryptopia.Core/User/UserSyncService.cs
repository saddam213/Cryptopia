using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.User;
using System;
using System.Threading.Tasks;
using System.Data.Entity;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.User
{
	public class UserSyncService : IUserSyncService
	{
		public IDataContextFactory DataContextFactory { get; set; }
		public IPoolDataContextFactory PoolDataContextFactory { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<IServiceResult> SyncUser(string userId)
		{
			try
			{
				if (string.IsNullOrEmpty(userId))
					return new ServiceResult(false);

				using (var hubContext = DataContextFactory.CreateContext())
				using (var poolContext = PoolDataContextFactory.CreateContext())
				using (var exchangeContext = ExchangeDataContextFactory.CreateContext())
				{
					var hubUser = await hubContext.Users.FirstOrDefaultAsync(x => x.Id == userId).ConfigureAwait(false);
					if (hubUser == null)
						return new ServiceResult(false);

					var exchangeUser =
						await exchangeContext.Users.FirstOrDefaultAsync(x => x.Id == new Guid(userId)).ConfigureAwait(false);
					if (exchangeUser == null)
					{
						exchangeUser = new Entity.User {Id = new Guid(userId)};
						exchangeContext.Users.Add(exchangeUser);
					}

					var poolUser = await poolContext.User.FirstOrDefaultAsync(x => x.Id == new Guid(userId)).ConfigureAwait(false);
					if (poolUser == null)
					{
						poolUser = new Entity.PoolUser {Id = new Guid(userId), IsEnabled = true};
						poolContext.User.Add(poolUser);
					}

					// Exchange user
					exchangeUser.ChatHandle = hubUser.ChatHandle;
					exchangeUser.DisableExchangeNotify = hubUser.DisableExchangeNotify;
					exchangeUser.DisableFaucetNotify = hubUser.DisableFaucetNotify;
					exchangeUser.DisableMarketplaceNotify = hubUser.DisableMarketplaceNotify;
					exchangeUser.DisablePoolNotify = hubUser.DisablePoolNotify;
					exchangeUser.DisableRewards = hubUser.DisableRewards;
					exchangeUser.DisableTipNotify = hubUser.DisableTipNotify;
					exchangeUser.DisableWithdrawEmailConfirmation = hubUser.DisableWithdrawEmailConfirmation;
					exchangeUser.Email = hubUser.Email;
					exchangeUser.IsDisabled = hubUser.IsDisabled;
					exchangeUser.MiningHandle = hubUser.MiningHandle;
					exchangeUser.Referrer = hubUser.Referrer;
					exchangeUser.RegisterDate = hubUser.RegisterDate;
					exchangeUser.TrustRating = hubUser.TrustRating;
					exchangeUser.UserName = hubUser.UserName;
					exchangeUser.IsUnsafeWithdrawEnabled = hubUser.IsUnsafeWithdrawEnabled;
					exchangeUser.IsApiUnsafeWithdrawEnabled = hubUser.IsApiUnsafeWithdrawEnabled;
					exchangeUser.VerificationLevel = hubUser.VerificationLevel;

					// Pool user
					poolUser.UserName = hubUser.UserName;
					poolUser.MiningHandle = hubUser.MiningHandle;
					poolUser.DisableNotifications = hubUser.DisablePoolNotify;

					await poolContext.SaveChangesAsync().ConfigureAwait(false);
					await exchangeContext.SaveChangesAsync().ConfigureAwait(false);

					return new ServiceResult(true);
				}
			}
			catch (Exception)
			{
				return new ServiceResult(false);
			}
		}
	}
}