using Cryptopia.Common.Address;
using Cryptopia.Common.Currency;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.Deposit;
using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;

namespace Cryptopia.Core.Address
{
	public class AddressReader : IAddressReader
	{
		public ICurrencyReader CurrencyReader { get; set; }
		public IDepositService DepositService { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<AddressModel> GetAddress(string userId, int currencyId)
		{
			var currency = await CurrencyReader.GetCurrency(currencyId);
			if (currency == null)
				return new AddressModel { ErrorMessage = "Currency not found." };

			if (currency.ListingStatus != Enums.CurrencyListingStatus.Active)
				return new AddressModel { ErrorMessage = $"Unable to generate address for {currency.Symbol}, Reason: {currency.ListingStatus}" };

			if (currency.Status != Enums.CurrencyStatus.OK)
				return new AddressModel { ErrorMessage = $"Unable to generate address for {currency.Symbol}, Reason: {currency.StatusMessage}" };

			var currentUser = new Guid(userId);
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var existingAddress = await GetAddressModel(context, currencyId, currentUser);
				if (existingAddress != null)
					return existingAddress;
			}

			var newAddress = await DepositService.GenerateAddress(currentUser, currencyId);
			if (newAddress == null || newAddress.Count < 2)
				return new AddressModel { ErrorMessage = "Failed to generate new address" };

			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var address = new Entity.Address
				{
					AddressHash = newAddress[0],
					PrivateKey = newAddress[1],
					UserId = currentUser,
					CurrencyId = currencyId
				};
				context.Address.Add(address);
				await context.SaveChangesAsync().ConfigureAwait(false);
				return await GetAddressModel(context, currencyId, currentUser);
			}
		}

		public async Task<DisplayAddressModel> GetDisplayAddress(string userId, string symbol)
		{
			var currentUser = new Guid(userId);
			var currency = await CurrencyReader.GetCurrency(symbol).ConfigureAwait(false);
			if (currency == null)
				return null;

			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var address = await context.Address
					.Where(x => x.CurrencyId == currency.CurrencyId && x.UserId == currentUser)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);

				return new DisplayAddressModel
				{
					AddressData = address?.AddressHash,
					AddressData2 = currency.BaseAddress,
					CurrencyId = currency.CurrencyId,
					CurrencyType = currency.Type,
					Name = currency.Name,
					Symbol = currency.Symbol,
					Instructions = currency.DepositInstructions,
					Message = currency.DepositMessage,
					MessageType = currency.DepositMessageType.ToString().ToLower(),
					QrFormat = currency.QrFormat,
					AddressType = currency.AddressType
				};
			}
		}

		private async Task<AddressModel> GetAddressModel(IExchangeDataContext context, int currency, Guid userId)
		{
			return await context.Address
					.Where(x => x.CurrencyId == currency && x.UserId == userId)
					.Select(x => new AddressModel
					{
						AddressData = x.AddressHash,
						AddressData2 = x.Currency.BaseAddress,
						CurrencyId = x.CurrencyId,
						CurrencyType = x.Currency.Type,
						Name = x.Currency.Name,
						Symbol = x.Currency.Symbol,
						QrFormat = x.Currency.Settings.QrFormat,
						AddressType = x.Currency.Settings.AddressType
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
		}
	}
}
