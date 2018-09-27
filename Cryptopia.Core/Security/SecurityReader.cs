using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Security;

namespace Cryptopia.Core.Security
{
	public class SecurityReader : ISecurityReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetLockedOutUsers(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var now = DateTime.UtcNow;
				var query = context.Users
					.AsNoTracking()
					.Where(u => !u.IsDisabled && u.LockoutEndDateUtc != null && u.LockoutEndDateUtc > now)
					.Select(user => new
					{
						UserName = user.UserName,
						Email = user.Email,
						EndTime = user.LockoutEndDateUtc,
					});
				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetUserLogons(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.UserLogons
					.AsNoTracking()
					.Select(logon => new
					{
						UserName = logon.User.UserName,
						IPAddress = logon.IPAddress,
						Timestamp = logon.Timestamp
					});
				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}
	}
}