using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.TradePair
{
	public interface ITradePairWriter
	{
		Task<IWriterResult> AdminCreateTradePair(CreateTradePairModel model);
		Task<IWriterResult> AdminUpdateTradePair(UpdateTradePairModel model);
	}
}
