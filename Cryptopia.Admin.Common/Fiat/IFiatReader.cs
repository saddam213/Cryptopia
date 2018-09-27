using Cryptopia.Infrastructure.Common.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.Fiat
{
	public interface IFiatReader
	{
		Task<DataTablesResponse> GetDeposits(DataTablesModel model);
		Task<DataTablesResponse> GetWithdrawals(DataTablesModel model);
	}
}
