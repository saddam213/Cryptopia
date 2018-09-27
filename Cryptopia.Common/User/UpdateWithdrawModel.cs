using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.User
{
	public class UpdateWithdrawModel
	{
		public bool AddressBookOnly { get; set; }
		public bool DisableConfirmation { get; set; }
		public bool HasWithdrawTfa { get; set; }
	}
}
