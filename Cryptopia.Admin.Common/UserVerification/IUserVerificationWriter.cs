namespace Cryptopia.Admin.Common.UserVerification
{
	using System.Threading.Tasks;
	using Cryptopia.Infrastructure.Common.Results;

	public interface IUserVerificationWriter
	{
		Task<IWriterResult> RejectUser(string currentAdminId, int verificationId, string reason);
		Task<IWriterResult> AcceptUser(int verificationId, string currentAdminId);
	}
}