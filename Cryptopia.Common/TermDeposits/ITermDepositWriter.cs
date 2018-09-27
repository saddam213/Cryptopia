using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.TermDeposits
{
	public interface ITermDepositWriter
	{
		Task<IWriterResult> CreateTermDeposit(string userId, CreateTermDepositModel model);
		Task<IWriterResult> CancelTermDeposit(string userId, int termDepositId);


		Task<IWriterResult> AdminUpdatePayment(string userId, UpdateTermDepositPaymentModel model);
	}
}
