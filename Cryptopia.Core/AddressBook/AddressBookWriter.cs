using Cryptopia.Common.Address;
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.Deposit;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.Results;
using Cryptopia.Base;


namespace Cryptopia.Core.Address
{
	public class AddressBookWriter : IAddressBookWriter
	{
		public IDepositService DepositService { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<IWriterResult> CreateAddressBook(string userId, AddressBookModel model)
		{
			try
			{
				var address = model.Address;
				if (model.AddressType == AddressType.Message || model.AddressType == AddressType.PayloadId || model.AddressType == AddressType.PaymentId)
					address = $"{model.Address}:{model.PaymentId ?? string.Empty}";

				if (model.CurrencyId == Constant.NZDT_ID && !string.IsNullOrEmpty(model.BankAccount))
				{
					address = $"{model.BankAccount}:{model.BankReference ?? string.Empty}";
					if (!Constant.NZDT_BANK_REGEX.IsMatch(address))
						return new WriterResult(false, "Invalid Bank Account Number");
				}

				if (model.AddressType == AddressType.PaymentId && !CurrencyExtensions.IsValidPaymentId(model.PaymentId))
					return new WriterResult(false, "Invalid {0} PaymentId", model.Symbol);

				if (!await DepositService.ValidateAddress(model.CurrencyId, address).ConfigureAwait(false))
				{
					return new WriterResult(false, "Invalid {0} Address", model.Symbol);
				}

				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var currentUser = new Guid(userId);
					var addressBook = new Entity.AddressBook
					{
						Address = address,
						CurrencyId = model.CurrencyId,
						IsEnabled = true,
						Label = model.Label,
						UserId = currentUser,
					};
					context.AddressBook.Add(addressBook);
					await context.SaveChangesAsync().ConfigureAwait(false);
					return new WriterResult(true);
				}
			}
			catch (Exception)
			{
				return new WriterResult(false, "Failed to validate {0} Address", model.Symbol);
			}
		}

		public async Task<IWriterResult> DeleteAddressBook(string userId, int addressbookId)
		{
			try
			{
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var currentUser = new Guid(userId);
					var addressBook =
						await
							context.AddressBook.FirstOrDefaultAsync(x => x.UserId == currentUser && x.Id == addressbookId)
								.ConfigureAwait(false);
					if (addressBook == null)
						return new WriterResult(false, "AddressBook item not found.");

					context.AddressBook.Remove(addressBook);
					await context.SaveChangesAsync().ConfigureAwait(false);
					return new WriterResult(true, "Successfully removed addressbook entry.");
				}
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}
	}
}
