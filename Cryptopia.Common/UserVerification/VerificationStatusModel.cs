using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Enums;

namespace Cryptopia.Common.UserVerification
{
	public class VerificationStatusModel
	{
		public decimal Current { get; set; }
		public VerificationLevel Level { get; set; }
		public decimal Limit { get; set; }
	}
}
