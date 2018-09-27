using Cryptopia.Common.Lotto;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Entity;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.Lotto
{
	public class LottoWriter : ILottoWriter
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<IWriterResult> CreateLottoItem(CreateLottoItemModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var lottoItem = new LottoItem
				{
					Name = model.Name,
					CurrencyId = model.CurrencyId,
					CurrentDrawId = 1,
					Description = model.Description,
					Fee = model.Fee,
					Hours = model.Hours,
					LottoType = model.LottoType,
					Prizes = model.Prizes,
					Rate = model.Rate,
					NextDraw = model.NextDraw,
					Status = model.Status,
					Expires = model.Expires
				};
				context.LottoItem.Add(lottoItem);
				await context.SaveChangesAsync().ConfigureAwait(false);

				return new WriterResult(true, "Successfully created lotto item.");
			}
		}

		public async Task<IWriterResult> UpdateLottoItem(UpdateLottoItemModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var lottoItem =
					await context.LottoItem.Where(x => x.Id == model.LottoItemId).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (lottoItem == null)
					return new WriterResult(false, "LottoItem #{0} not found", model.LottoItemId);

				lottoItem.Name = model.Name;
				lottoItem.Description = model.Description;
				lottoItem.LottoType = model.LottoType;
				lottoItem.Status = model.Status;
				await context.SaveChangesAsync().ConfigureAwait(false);

				return new WriterResult(true, "Successfully updated lotto item.");
			}
		}

		public async Task<IWriterResult> DeleteLottoItem(int lottoItemId)
		{
			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var lottoItem =
					await
						context.LottoItem.Where(x => x.Id == lottoItemId && x.Status != Enums.LottoItemStatus.Active)
							.FirstOrDefaultNoLockAsync()
							.ConfigureAwait(false);
				if (lottoItem == null)
					return new WriterResult(false, "LottoItem #{0} not found or is currently active", lottoItemId);

				context.LottoHistory.RemoveRange(context.LottoHistory.Where(x => x.LottoItemId == lottoItemId));
				context.LottoTicket.RemoveRange(context.LottoTicket.Where(x => x.LottoItemId == lottoItemId));
				context.LottoItem.Remove(lottoItem);
				await context.SaveChangesAsync().ConfigureAwait(false);

				return new WriterResult(true, "Successfully deleted lotto item.");
			}
		}
	}
}