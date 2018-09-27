namespace Cryptopia.Admin.Common.UserVerification
{
	using System.Threading.Tasks;
	using Cryptopia.Infrastructure.Common.DataTables;

	public interface IUserVerificationReader
	{
		Task<DataTablesResponse> GetUserVerifications(DataTablesModel model);
		Task<DataTablesResponse> GetRejectedUserVerifications(DataTablesModel model);
		Task<DataTablesResponse> GetCompletedVerifications(DataTablesModel model);
		Task<UserDetailsModel> GetUserDetails(int id);
		Task<UserDetailsModel> GetRejectedUserDetails(int id);
		Task<UserDetailsModel> GetCompletedUserDetails(int id);
	}
}