using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Common.Deposit
{
	public interface IDepositReader
	{
		Task<DataTablesResponse> AdminGetDeposits(string userId, DataTablesModel model);
	}
}