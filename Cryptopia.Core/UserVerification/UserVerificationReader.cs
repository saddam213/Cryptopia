using Cryptopia.Common.Balance;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.UserVerification;
using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;

namespace Cryptopia.Core.Verification
{
	public class UserVerificationReader : IUserVerificationReader
	{
		public IDataContextFactory DataContextFactory { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }
		public IBalanceEstimationService BalanceEstimationService { get; set; }

		public async Task<VerificationStatusModel> GetVerificationStatus(string userId)
		{
			var currentUser = new Guid(userId);
			var lastTime = DateTime.UtcNow.AddHours(-24);
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == currentUser);
				if (user == null)
					return null;

				var current = 0m;
				if (user.VerificationLevel != VerificationLevel.Legacy)
				{
					var currentWithdrawDetails = await context.Withdraw
						.Where(x => x.UserId == currentUser && x.Status != Enums.WithdrawStatus.Canceled && x.TimeStamp > lastTime)
						.ToListNoLockAsync();
					if (currentWithdrawDetails.Any())
						current = currentWithdrawDetails.Sum(x => x.EstimatedPrice);

					var currentTransferDetails = await context.Transfer
						.Where(x => x.UserId == currentUser && x.Timestamp > lastTime && (x.TransferType == TransferType.User || x.TransferType == TransferType.Tip))
						.ToListNoLockAsync();
					if (currentTransferDetails.Any())
						current += currentTransferDetails.Sum(x => x.EstimatedPrice);
				}

				return new VerificationStatusModel
				{
					Level = user.VerificationLevel,
					Limit = VerificationLimit(user.VerificationLevel),
					Current = current
				};
			}
		}

		public async Task<UserVerificationModel> GetVerification(string userId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == userId);
				if (user == null)
					return null;

				var result = new UserVerificationModel
				{
					Email = user.Email,
					VerificationLevel = user.VerificationLevel
				};

				var userVerification = await context.UserVerification.FirstOrDefaultNoLockAsync(x => x.UserId == userId);
				if (userVerification != null)
				{
					result.FirstName = userVerification.FirstName;
					result.LastName = userVerification.LastName;
					result.Birthday = userVerification.Birthday;
					result.Gender = userVerification.Gender;
					result.Address = userVerification.Address;
					result.City = userVerification.City;
					result.State = userVerification.State;
					result.Postcode = userVerification.Postcode;
					result.Country = userVerification.Country;
				}
				return result;
			}
		}

		private decimal VerificationLimit(VerificationLevel level)
		{
			switch (level)
			{
				case VerificationLevel.Legacy:
					return 0;
				case VerificationLevel.Level1:
				case VerificationLevel.Level1Pending:
				case VerificationLevel.Level2Pending:
				case VerificationLevel.Level3Pending:
					return Constant.VERIFICATION_WITHDRAW_LEVEL1_LIMIT;
				case VerificationLevel.Level2:
					return Constant.VERIFICATION_WITHDRAW_LEVEL2_LIMIT;
				case VerificationLevel.Level3:
					return Constant.VERIFICATION_WITHDRAW_LEVEL3_LIMIT;
				default:
					break;
			}
			return 0;
		}

		public bool IsVerified(VerificationLevel level)
		{
			switch (level)
			{
			
				case VerificationLevel.Level1:
				case VerificationLevel.Level1Pending:
				case VerificationLevel.Level2Pending:
				case VerificationLevel.Level3Pending:
					return false;
				case VerificationLevel.Level2:
				case VerificationLevel.Level3:
				case VerificationLevel.Legacy:
					return true;
				default:
					break;
			}
			return false;
		}
	}
}
