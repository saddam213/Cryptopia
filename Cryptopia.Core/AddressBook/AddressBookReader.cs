using Cryptopia.Common.Address;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;

namespace Cryptopia.Core.Address
{
	public class AddressBookReader : IAddressBookReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<bool> AddressBookEntryExists(string userId, int currencyId, string address)
		{
			var currentUser = new Guid(userId);
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				return await context.AddressBook
					.AsNoTracking()
					.Where(x => x.UserId == currentUser && x.CurrencyId == currencyId && x.Address == address && x.IsEnabled)
					.AnyNoLockAsync().ConfigureAwait(false);
			}
		}

		public async Task<List<AddressBookModel>> GetAddressBook(string userId, int currencyId)
		{
			try
			{
				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var query = await context.AddressBook
						.AsNoTracking()
						.Where(x => x.CurrencyId == currencyId && x.UserId == currentUser && x.IsEnabled)
						.Select(a => new AddressBookModel
						{
							Address = a.Address,
							CurrencyId = a.CurrencyId,
							Id = a.Id,
							Label = a.Label,
							Symbol = a.Currency.Symbol
						}).ToListNoLockAsync().ConfigureAwait(false);

					return query;
				}
			}
			catch (Exception)
			{
				return new List<AddressBookModel>();
			}
		}

		public async Task<DataTablesResponse> GetAddressBookDataTable(string userId, DataTablesModel model)
		{
			try
			{
				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var query = context.AddressBook
						.AsNoTracking()
						.Where(x => x.UserId == currentUser && x.IsEnabled)
						.Select(a => new
						{
							Id = a.Id,
							CurrencyId = a.CurrencyId,
							Symbol = a.Currency.Symbol,
							Label = a.Label,
							Address = a.Address,
							AddressType = a.Currency.Settings.AddressType
						});

					return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
				}
			}
			catch (Exception)
			{
				return model.GetEmptyDataTableResult();
			}
		}
	}
}
