using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.Chat;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Cache;

namespace Cryptopia.Core.Chat
{
	public class ChatReader : IChatReader
	{
		public ICacheService CacheService { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetBannedUsers(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Users
					.AsNoTracking()
					.Where(u => !u.IsDisabled && u.ChatBanEndTime > DateTime.UtcNow)
					.Select(user => new
					{
						//UserId = user.Id,
						ChatHandle = user.ChatHandle,
						//UserName = user.UserName,
						BanEnds = user.ChatBanEndTime,
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetChatMessages(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.ChatMessages
					.AsNoTracking()
					.Select(message => new
					{
						message.Id,
						message.User.ChatHandle,
						message.User.UserName,
						message.Message,
						message.Timestamp,
						message.IsEnabled
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<List<ChatUserModel>> GetChatUsers()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var lastTimestamp = DateTime.UtcNow.AddHours(-24);
				return await context.Users
						.AsNoTracking()
						.Where(x => x.ChatBanEndTime > DateTime.UtcNow || x.ChatMessages.Any(m => m.Timestamp > lastTimestamp))
						.Select(x => new ChatUserModel
						{
							UserId = x.Id,
							ChatHandle = x.ChatHandle,
							UserName = x.UserName
						}).OrderBy(x => x.ChatHandle)
						.ToListNoLockAsync().ConfigureAwait(false);
			}
		}


	}
}