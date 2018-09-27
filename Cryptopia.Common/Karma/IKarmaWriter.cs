using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.Karma
{
	public interface IKarmaWriter
	{
		Task<IWriterResult> CreateKarma(string userId, CreateKarmaModel model);
		Task<IWriterResult> SpendKarma(string userId, SpendKarmaModel model);
	}
}
