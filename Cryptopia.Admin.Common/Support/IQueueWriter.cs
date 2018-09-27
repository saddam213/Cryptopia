using Cryptopia.Infrastructure.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.Support
{
	public interface IQueueWriter
	{
		Task<IWriterResult> CreateQueue(string adminUserId, string name);
		Task<IWriterResult> UpdateQueue(SupportQueueModel model);
		Task<IWriterResult> DeleteQueue(string adminUserId, SupportQueueModel model);
	}
}
