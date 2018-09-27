using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Admin.Common.Approval
{
	public interface IAdminApprovalReader
	{
		Task<AdminApprovalModel> GetApproval(int approvalId);
		Task<DataTablesResponse> GetApprovals(DataTablesModel model);
	}
}