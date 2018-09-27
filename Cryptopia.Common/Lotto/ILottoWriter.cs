using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.Lotto
{
	public interface ILottoWriter
	{
		Task<IWriterResult> CreateLottoItem(CreateLottoItemModel model);
		Task<IWriterResult> UpdateLottoItem(UpdateLottoItemModel model);
		Task<IWriterResult> DeleteLottoItem(int lottoItemId);
	}
}
