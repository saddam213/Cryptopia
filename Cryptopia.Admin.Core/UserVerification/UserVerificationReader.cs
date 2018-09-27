namespace Cryptopia.Admin.Core.UserVerification
{
	using System.Linq;
	using System.Threading.Tasks;
	using Cryptopia.Admin.Common.UserVerification;
	using Cryptopia.Enums;
	using Cryptopia.Infrastructure.Common.DataContext;
	using Cryptopia.Infrastructure.Common.DataTables;

	public class UserVerificationReader : IUserVerificationReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetUserVerifications(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.UserVerification
					.Where(x => x.User.VerificationLevel == VerificationLevel.Level2Pending)
					.Select(uv => new
				{
					uv.Id,
					User = uv.User.UserName,
					Requested = uv.Timestamp
				}).OrderByDescending(x => x.Requested);

				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		public async Task<DataTablesResponse> GetRejectedUserVerifications(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.UserVerificationReject.Select(uv => new
				{
					uv.Id,
					User = uv.User.UserName,
					RejectedOn = uv.Timestamp
				}).OrderByDescending(x => x.RejectedOn);

				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		public async Task<DataTablesResponse> GetCompletedVerifications(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.UserVerification
					.Where(x => x.User.VerificationLevel == VerificationLevel.Level2)
					.Select(uv => new
					{
						uv.Id,
						User = uv.User.UserName,
						Requested = uv.Timestamp,
						uv.Approved,
						ApprovedBy = uv.ApprovedByUser == null ? string.Empty : uv.ApprovedByUser.UserName,
					}).OrderByDescending(x => x.Requested);

				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		public async Task<UserDetailsModel> GetUserDetails(int id)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.UserVerification
					.Where(uv => uv.Id == id)
					.Select(uv => new UserDetailsModel
					{
						VerificationId = uv.Id, 
						FirstName = uv.FirstName,
						LastName = uv.LastName,
						Address = uv.Address,
						Email = uv.User.Email,
						Identification1 = uv.Identification1,
						Identification2 = uv.Identification2,
						Birthday = uv.Birthday,
						City = uv.City,
						Country = uv.Country,
						Gender = uv.Gender,
						State = uv.State,
						Postcode = uv.Postcode,
						AdminCanVerify = uv.User.VerificationLevel == VerificationLevel.Level2Pending
					}).FirstOrDefaultNoLockAsync();
			}
		}

		public async Task<UserDetailsModel> GetRejectedUserDetails(int id)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.UserVerificationReject
					.Where(uv => uv.Id == id)
					.Select(uv => new UserDetailsModel
					{
						VerificationId = uv.Id,
						FirstName = uv.FirstName,
						LastName = uv.LastName,
						Address = uv.Address,
						Email = uv.User.Email,
						Identification1 = uv.Identification1,
						Identification2 = uv.Identification2,
						Birthday = uv.Birthday,
						City = uv.City,
						Country = uv.Country,
						Gender = uv.Gender,
						State = uv.State,
						Postcode = uv.Postcode,
						AdminCanVerify = false, 
						RejectReason = uv.RejectReason
					}).FirstOrDefaultNoLockAsync();
			}
		}

		public async Task<UserDetailsModel> GetCompletedUserDetails(int id)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.UserVerification
					.Where(uv => uv.Id == id)
					.Select(uv => new UserDetailsModel
					{
						VerificationId = uv.Id,
						FirstName = uv.FirstName,
						LastName = uv.LastName,
						Address = uv.Address,
						Email = uv.User.Email,
						Identification1 = uv.Identification1,
						Identification2 = uv.Identification2,
						Birthday = uv.Birthday,
						City = uv.City,
						Country = uv.Country,
						Gender = uv.Gender,
						State = uv.State,
						Postcode = uv.Postcode,
						AdminCanVerify = false
					}).FirstOrDefaultNoLockAsync();
			}
		}
	}
}