using Cryptopia.Admin.Common.Security;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Core.Security
{
	public class AdminSecurityReader : IAdminSecurityReader
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
	}
}
