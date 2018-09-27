using Cryptopia.Infrastructure.Common.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Referral
{
	public interface IReferralReader
	{
		Task<ReferralInfoModel> GetActiveReferral(string userId);
		Task<DataTablesResponse> GetHistory(string userId, DataTablesModel model);
		Task<DataTablesResponse> AdminGetHistory(DataTablesModel model);
	}
}
