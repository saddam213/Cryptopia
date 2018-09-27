using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.Mineshaft
{
	public interface IMineshaftWriter
	{
		Task<IWriterResult> ChangeUserPool(string userId, ChangePoolModel model);
	}
}
