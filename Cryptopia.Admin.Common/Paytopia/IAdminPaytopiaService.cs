using Cryptopia.Common.Paytopia;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Infrastructure.Common.Results;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.Paytopia
{
	public interface IAdminPaytopiaService
	{
		Task<DataTablesResponse> GetPayments(DataTablesModel model);
		Task<PaytopiaPaymentModel> GetPayment(int id);
		Task<IWriterResult> UpdatePaytopiaPayment(string adminUserId, AdminUpdatePaytopiaPaymentModel model);
	}
}
