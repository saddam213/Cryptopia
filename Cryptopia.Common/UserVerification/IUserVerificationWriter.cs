using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.UserVerification
{
	public interface IUserVerificationWriter
	{
		Task<IWriterResult> CreateVerification(string userId, UserVerificationModel model);
		Task<IWriterResult> AdminApproveVerification(string userId, ApproveVerificationModel model);
	}
}
