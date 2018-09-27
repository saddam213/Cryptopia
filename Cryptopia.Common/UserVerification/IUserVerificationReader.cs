using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.UserVerification
{
	public interface IUserVerificationReader
	{
		bool IsVerified(VerificationLevel level);
		Task<UserVerificationModel> GetVerification(string userId);
		Task<VerificationStatusModel> GetVerificationStatus(string userId);
	}
}
