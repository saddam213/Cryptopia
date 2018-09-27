using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Admin.Common.AdminUser;
using Cryptopia.Common.DataContext;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Admin.Core.User
{
	public class AdminUserReader : IAdminUserReader
	{
		public IDataContextFactory DataContextFactory { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<AdminUserDetailsModel> GetUserDetails(string username)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var data = await context.Users
					.Include(x => x.Profile).DefaultIfEmpty()
					.Include(x => x.Settings).DefaultIfEmpty()
					.Where(u => u.UserName == username)
					.Select(user => new AdminUserDetailsModel
					{
						Email = user.Email,
						UserName = user.UserName,
						RoleCss = user.RoleCss,
						TrustRating = user.TrustRating,
						ShareCount = user.ShareCount,
						KarmaTotal = user.KarmaTotal,
						EmailConfirmed = user.EmailConfirmed,
						DisableRewards = user.DisableRewards,
						DisableTips = user.DisableTips,
						LockoutEnd = user.LockoutEndDateUtc,
						MiningHandle = user.MiningHandle,
						ChatHandle = user.ChatHandle,
						ChatDisableEmoticons = user.ChatDisableEmoticons,
						Theme = user.Settings != null ? user.Settings.Theme.ToString() : SiteTheme.Light.ToString(),
						Referrer = user.Referrer,
						RegisterDate = user.RegisterDate,
						IsDisabled = user.IsDisabled,
						VerificationLevel = user.VerificationLevel.ToString(),
						AboutMe = user.Profile != null ? user.Profile.AboutMe : "",
						Address = user.Profile != null ? user.Profile.Address : "",
						Birthday = user.Profile != null ? user.Profile.Birthday : DateTime.UtcNow,
						City = user.Profile != null ? user.Profile.City : "",
						ContactEmail = user.Profile != null ? user.Profile.ContactEmail : "",
						Country = user.Profile != null ? user.Profile.Country : "",
						Education = user.Profile != null ? user.Profile.Education : "",
						Facebook = user.Profile != null ? user.Profile.Facebook : "",
						FirstName = user.Profile != null ? user.Profile.FirstName : "",
						Gender = user.Profile != null ? user.Profile.Gender : "",
						Hobbies = user.Profile != null ? user.Profile.Hobbies : "",
						LastName = user.Profile != null ? user.Profile.LastName : "",
						LinkedIn = user.Profile != null ? user.Profile.LinkedIn : "",
						Occupation = user.Profile != null ? user.Profile.Occupation : "",
						Postcode = user.Profile != null ? user.Profile.Postcode : "",
						State = user.Profile != null ? user.Profile.State : "",
						Twitter = user.Profile != null ? user.Profile.Twitter : "",
						Website = user.Profile != null ? user.Profile.Website : "",
						IsPublic = user.Profile != null ? user.Profile.IsPublic : false,
					}).FirstOrDefaultNoLockAsync();
				if (data == null)
					return null;

				data.IsLocked = data.LockoutEnd > DateTime.UtcNow;
				return data;
			}
		}

		public async Task<AdminUserSecurityModel> GetUserSecurity(string username)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var data = await context.Users
					.AsNoTracking()
					.Include(x => x.TwoFactor)
					.Where(u => u.UserName == username)
					.Select(user => new AdminUserSecurityModel
					{
						UserName = user.UserName,
						DisableLogonEmail = user.DisableLogonEmail,
						IsApiEnabled = user.IsApiEnabled,
						IsApiWithdrawEnabled = user.IsApiWithdrawEnabled,
						IsUnsafeWithdrawEnabled = user.IsUnsafeWithdrawEnabled,
						IsApiUnsafeWithdrawEnabled = user.IsApiUnsafeWithdrawEnabled,
						DisableWithdrawEmailConfirmation = user.DisableWithdrawEmailConfirmation,
						TwoFactor = user.TwoFactor.Select(tfa => new AdminUserTwoFactorItemModel
						{
							Name = tfa.Component.ToString(),
							Component = tfa.Component,
							Type = tfa.Type.ToString()
						}).ToList(),
					}).FirstOrDefaultNoLockAsync();
				if (data == null)
					return null;


				foreach (var item in Enum.GetValues(typeof (Enums.TwoFactorComponent)).OfType<Enums.TwoFactorComponent>())
				{
					if (data.TwoFactor.Any(x => x.Component == item))
						continue;
					data.TwoFactor.Add(new AdminUserTwoFactorItemModel {Component = item, Name = item.ToString(), Type = "None"});
				}
				data.TwoFactor = data.TwoFactor.OrderBy(x => x.Component).ToList();
				return data;
			}
		}


		public async Task<AdminUserUpdateModel> GetUserUpdate(string username)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.UserName == username);
				if (user == null)
					return null;

				return new AdminUserUpdateModel
				{
					Id = user.Id,
					UserName = user.UserName,
					EmailConfirmed = user.EmailConfirmed,
					RoleCss = user.RoleCss,
					ShareCount = user.ShareCount
				};
			}
		}

		public async Task<AdminUserApiUpdateModel> GetApiUpdate(string username)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.UserName == username);
				if (user == null)
					return null;

				return new AdminUserApiUpdateModel
				{
					Id = user.Id,
					UserName = user.UserName,
					IsApiEnabled = user.IsApiEnabled,
					IsApiUnsafeWithdrawEnabled = user.IsApiUnsafeWithdrawEnabled,
					IsApiWithdrawEnabled = user.IsApiWithdrawEnabled
				};
			}
		}

		public async Task<AdminUserProfileUpdateModel> GetProfileUpdate(string username)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var user = await context.Users
					.Include(x => x.Profile)
					.FirstOrDefaultNoLockAsync(x => x.UserName == username);
				if (user == null)
					return null;

				return new AdminUserProfileUpdateModel
				{
					Id = user.Id,
					UserName = user.UserName,
					AboutMe = user.Profile.AboutMe,
					Address = user.Profile.Address,
					Birthday = user.Profile.Birthday,
					City = user.Profile.City,
					ContactEmail = user.Profile.ContactEmail,
					Country = user.Profile.Country,
					Education = user.Profile.Education,
					Facebook = user.Profile.Facebook,
					FirstName = user.Profile.FirstName,
					Gender = user.Profile.Gender,
					Hobbies = user.Profile.Hobbies,
					LastName = user.Profile.LastName,
					LinkedIn = user.Profile.LinkedIn,
					Occupation = user.Profile.Occupation,
					Postcode = user.Profile.Postcode,
					State = user.Profile.State,
					Twitter = user.Profile.Twitter,
					Website = user.Profile.Website,
					IsPublic = user.Profile.IsPublic,
				};
			}
		}

		public async Task<AdminUserSettingsUpdateModel> GetSettingsUpdate(string username)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var user = await context.Users
					.Include(x => x.Settings)
					.FirstOrDefaultNoLockAsync(x => x.UserName == username);
				if (user == null)
					return null;

				return new AdminUserSettingsUpdateModel
				{
					Id = user.Id,
					UserName = user.UserName,
					HideZeroBalance = user.Settings.HideZeroBalance,
					ShowFavoriteBalance = user.Settings.ShowFavoriteBalance,
					ChatDisableEmoticons = user.ChatDisableEmoticons,
					DisableLogonEmail = user.DisableLogonEmail,
					DisableRewards = user.DisableRewards,
					DisableTips = user.DisableTips,
					Theme = user.Settings.Theme
				};
			}
		}


		public async Task<DataTablesResponse> GetUserDeposits(string username, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Deposit
					.AsNoTracking()
					.Where(x => x.User.UserName == username && x.Type == DepositType.Normal)
					.Select(x => new
					{
						Id = x.Id,
						Currency = x.Currency.Symbol,
						Amount = x.Amount,
						Status = x.Status,
						TxId = x.Txid,
						Conf = x.Confirmations,
						Timestamp = x.TimeStamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetUserWithdraw(string username, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Withdraw
					.AsNoTracking()
					.Where(x => x.User.UserName == username)
					.Select(x => new
					{
						Id = x.Id,
						Currency = x.Currency.Symbol,
						Amount = x.Amount,
						Status = x.Status,
						Confirmed = x.Confirmed,
						TxId = x.Txid,
						Address = x.Address,
						Conf = x.Confirmations,
						Timestamp = x.TimeStamp,
						Init = x.IsApi ? "API" : "UI"
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetUserTransfer(string username, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.UserName == username);
				if (user == null)
					return model.GetEmptyDataTableResult();

				var query = context.Transfer
					.AsNoTracking()
					.Where(x => x.UserId == user.Id || x.ToUserId == user.Id)
					.Select(x => new
					{
						Id = x.Id,
						Currency = x.Currency.Symbol,
						Sender = x.User.UserName,
						Receiver = x.ToUser.UserName,
						Amount = x.Amount,
						Type = x.TransferType,
						Timestamp = x.Timestamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetUserTrades(string username, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.UserName == username);
				if (user == null)
					return model.GetEmptyDataTableResult();

				var query = context.Trade
					.AsNoTracking()
					.Where(x => x.UserId == user.Id)
					.Select(x => new
					{
						Id = x.Id,
						TradePair = string.Concat(x.TradePair.Currency1.Symbol, "/", x.TradePair.Currency2.Symbol),
						Type = x.Type,
						Amount = x.Amount,
						Remaining = x.Remaining,
						Price = x.Rate,
						Fee = x.Fee,
						Status = x.Status,
						Timestamp = x.Timestamp,
						Init = x.IsApi ? "API" : "UI"
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetUserTradeHistory(string username, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.UserName == username);
				if (user == null)
					return model.GetEmptyDataTableResult();

				var query = context.TradeHistory
					.AsNoTracking()
					.Where(x => x.UserId == user.Id || x.ToUserId == user.Id)
					.Select(x => new
					{
						Id = x.Id,
						TradePair = string.Concat(x.TradePair.Currency1.Symbol, "/", x.TradePair.Currency2.Symbol),
						Type = x.ToUser.UserName == username ? TradeHistoryType.Sell : TradeHistoryType.Buy,
						Amount = x.Amount,
						Rate = x.Rate,
						Fee = x.Fee,
						Total = ((double) x.Amount*(double) x.Rate) - (double) x.Fee,
						Timestamp = x.Timestamp,
						Init = x.IsApi ? "API" : "UI"
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetOpenTickets(string username, DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.UserName == username);
				if (user == null)
					return model.GetEmptyDataTableResult();

				var query = context.SupportTicket
					.AsNoTracking()
					.Where(x => x.UserId == user.Id && x.Status != SupportTicketStatus.Closed)
					.Select(x => new
					{
						Id = x.Id,
						Category = x.Category.ToString(),
						Title = x.Title,
						Status = x.Status.ToString(),
						Created = x.Created,
						Queue = x.Queue.Name
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetUserBalances(string username, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Balance
					.AsNoTracking()
					.Where(x => x.User.UserName == username)
					.Select(x => new
					{
						Id = x.Id,
						Currency = x.Currency.Symbol,
						Total = x.Total,
						Available = x.Total - (x.Unconfirmed + x.PendingWithdraw + x.HeldForTrades),
						HeldForTrades = x.HeldForTrades,
						PendingWithdraw = x.PendingWithdraw,
						Unconfirmed = x.Unconfirmed
					}).Distinct();

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetUserAddresses(string username, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Address
					.AsNoTracking()
					.Where(x => x.User.UserName == username)
					.Select(x => new
					{
						Id = x.Id,
						Currency = x.Currency.Symbol,
						x.AddressHash
					}).Distinct();

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetUserLogins(string username, DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.UserLogons
					.AsNoTracking()
					.Where(x => x.User.UserName == username)
					.Select(x => new
					{
						Currency = x.IPAddress,
						Amount = x.Timestamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}
	}
}