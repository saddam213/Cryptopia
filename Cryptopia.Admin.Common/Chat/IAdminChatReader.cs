using Cryptopia.Common.Chat;
using Cryptopia.Infrastructure.Common.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.Chat
{
	public interface IAdminChatReader
	{
		Task<DataTablesResponse> GetBannedUsers(DataTablesModel model);
	}
}
