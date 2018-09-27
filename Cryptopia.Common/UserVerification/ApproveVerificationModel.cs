using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.UserVerification
{
	public class ApproveVerificationModel
	{
		public int VerificationId { get; set; }
		public VerificationLevel VerificationLevel { get; set; }
	}
}
