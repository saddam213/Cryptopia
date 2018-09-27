using Cryptopia.Admin.Common.AdminCurrency;
using Cryptopia.Common.Cache;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.TradePair;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Core.TradePair
{
	public class AdminTradePairReader : IAdminTradePairReader
	{
		public ICacheService CacheService { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetTradePairs(DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.TradePair
				.AsNoTracking()
				.Select(x => new
				{
					Id = x.Id,
					Name = string.Concat(x.Currency1.Symbol, "_", x.Currency2.Symbol),
					Currency = x.Currency1.Name,
					BaseCurrency = x.Currency2.Name,
					Status = x.Status,
					StatusMessage = x.StatusMessage,
				});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<UpdateTradePairModel> GetTradePair(int id)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				return await context.TradePair
				.AsNoTracking()
				.Select(x => new UpdateTradePairModel
				{
					Id = x.Id,
					Name = string.Concat(x.Currency1.Symbol, "_", x.Currency2.Symbol),
					Status = x.Status,
					StatusMessage = x.StatusMessage
				}).FirstOrDefaultNoLockAsync(x => x.Id == id).ConfigureAwait(false);
			}
		}

	}
}
