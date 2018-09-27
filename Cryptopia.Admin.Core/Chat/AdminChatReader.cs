using Cryptopia.Admin.Common.Chat;
using Cryptopia.Common.Cache;
using Cryptopia.Common.Chat;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Core.Chat
{
	public class AdminChatReader : IAdminChatReader
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

	}
}
