using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Mineshaft
{
	public interface IMineshaftReader
	{
		Task<MineshaftSummary> GetMineshaftSummary();

		Task<DataTablesResponse> GetMineshaftSummary(DataTablesModel model, AlgoType? algoType);

		Task<BlockChartModel> GetBlockChart(int poolId);
		Task<HashrateChartModel> GetHashrateChart(int poolId, string userId);
		Task<DataTablesResponse> GetMiners(int poolId, DataTablesModel param);

		Task<MineshaftInfoModel> GetMineshaftInfo(int poolId);
		Task<MineshaftUserInfoModel> GetMineshaftUserInfo(string userId, int poolId);
	}
}
