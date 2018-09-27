using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Admin.Common.AdminCurrency;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Enums;
using Cryptopia.Common.Currency;
using System.Collections.Generic;

namespace Cryptopia.Admin.Core.Currency
{
	public class AdminCurrencyReader : IAdminCurrencyReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<AdminCurrencyInfoModel> GetCurrency(string symbol)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var summary = await context.Currency
					.AsNoTracking()
					.Include(x => x.Info)
					.Where(t => t.Symbol == symbol)
					.Select(currency => new AdminCurrencyInfoModel
					{
						Id = currency.Id,
						Name = currency.Name,
						Symbol = currency.Symbol,
						Connections = currency.Connections,
						Status = currency.Status.ToString(),
						StatusMessage = currency.StatusMessage,
						ListingStatus = currency.ListingStatus.ToString(),
						Version = currency.Version.ToString(),
						AlgoType = currency.Info.AlgoType.ToString(),
						Block = currency.Block,
						Confirmations = currency.MinConfirmations,
						NetworkType = currency.Info.NetworkType.ToString(),
					}).OrderBy(c => c.Name)
					.FirstOrDefaultNoLockAsync();
				return summary;
			}
		}

		public async Task<AdminCurrencyInfoModel> GetCurrency(int currencyId)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var summary = await context.Currency
					.AsNoTracking()
					.Include(x => x.Info)
					.Where(t => t.Id == currencyId)
					.Select(currency => new AdminCurrencyInfoModel
					{
						Id = currency.Id,
						Name = currency.Name,
						Symbol = currency.Symbol,
						Connections = currency.Connections,
						Status = currency.Status.ToString(),
						StatusMessage = currency.StatusMessage,
						ListingStatus = currency.ListingStatus.ToString(),
						Version = currency.Version.ToString(),
						AlgoType = currency.Info.AlgoType.ToString(),
						Block = currency.Block,
						Confirmations = currency.MinConfirmations,
						NetworkType = currency.Info.NetworkType.ToString(),
					}).OrderBy(c => c.Name)
					.FirstOrDefaultNoLockAsync();
				return summary;
			}
		}
		
		public async Task<DataTablesResponse> GetCurrencies(DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Currency
					.AsNoTracking()
					.Where(x => x.IsEnabled)
					.Select(x => new
					{
						Id = x.Id,
						Currency = x.Symbol,
						Amount = x.Name,
						Type = x.Type
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<UpdateCurrencyModel> GetUpdateCurrency(int currencyId)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				return await context.Currency
					.AsNoTracking()
					.Where(c => c.Id == currencyId)
					.Select(currency => new UpdateCurrencyModel
					{
						Id = currency.Id,
						Name = currency.Name,
						Symbol = currency.Symbol,
						PoolFee = currency.PoolFee,
						TradeFee = currency.TradeFee,
						WithdrawFee = currency.WithdrawFee,
						WithdrawFeeType = currency.WithdrawFeeType,
						WithdrawMin = currency.MinWithdraw,
						WithdrawMax = currency.MaxWithdraw,
						TipMin = currency.MinTip,
						MinBaseTrade = currency.MinBaseTrade,
						MinConfirmations = currency.MinConfirmations,
						Status = currency.Status,
						StatusMessage = currency.StatusMessage,
						ListingStatus = currency.ListingStatus,
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
			}
		}

		public async Task<UpdateListingStatusModel> GetUpdateListingStatusModel(int currencyId)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				return await context.Currency
					.AsNoTracking()
					.Where(c => c.Id == currencyId)
					.Select(currency => new UpdateListingStatusModel
					{
						CurrencyId = currency.Id,
						Name = currency.Name,
						Symbol = currency.Symbol,
						Status = currency.Status,
						StatusMessage = currency.StatusMessage,
						ListingStatus = currency.ListingStatus,
						DelistOn = currency.Settings.DelistOn
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetDeposits(int currencyId, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Deposit
					.AsNoTracking()
					.Where(x => x.CurrencyId == currencyId && x.Type == DepositType.Normal)
					.Select(x => new
					{
						Id = x.Id,
						UserName = x.User.UserName,
						Amount = x.Amount,
						Status = x.Status,
						TxId = x.Txid,
						Conf = x.Confirmations,
						Timestamp = x.TimeStamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetTransfers(int currencyId, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Transfer
					.AsNoTracking()
					.Where(x => x.CurrencyId == currencyId)
					.Select(x => new
					{
						Id = x.Id,
						Sender = x.User.UserName,
						Receiver = x.ToUser.UserName,
						Amount = x.Amount,
						Type = x.TransferType,
						Timestamp = x.Timestamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetWithdrawals(int currencyId, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Withdraw
					.AsNoTracking()
					.Where(x => x.CurrencyId == currencyId)
					.Select(x => new
					{
						Id = x.Id,
						UserName = x.User.UserName,
						Amount = x.Amount,
						Status = x.Status,
						Confirmed = x.Confirmed,
						TxId = x.Txid,
						Address = x.Address,
						Conf = x.Confirmations,
						Timestamp = x.TimeStamp,
						Init = x.IsApi ? "API" : "UI"
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetAddresses(int currencyId, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Address
					.AsNoTracking()
					.Where(x => x.CurrencyId == currencyId)
					.Select(x => new
					{
						Id = x.Id,
						UserName = x.User.UserName,
						x.AddressHash
					}).Distinct();

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<List<CurrencyModel>> GetCurrencies()
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var summary = await context.Currency
					.AsNoTracking()
					.Where(t => t.IsEnabled)
					.Select(currency => new CurrencyModel
					{
						CurrencyId = currency.Id,
						Name = currency.Name,
						Symbol = currency.Symbol,
						Connections = currency.Connections,
						Status = currency.Status,
						StatusMessage = currency.StatusMessage,
						ListingStatus = currency.ListingStatus,
						Version = currency.Version,
						AlgoType = currency.Info.AlgoType,
						BaseAddress = currency.BaseAddress,
						Block = currency.Block,
						BlockTime = currency.Info.BlockTime,
						Errors = currency.Errors,
						FeaturedExpires = currency.FeaturedExpires,
						TippingExpires = currency.TippingExpires,
						RewardsExpires = currency.RewardsExpires,
						MinBaseTrade = currency.MinBaseTrade,
						MinConfirmations = currency.MinConfirmations,
						NetworkType = currency.Info.NetworkType,
						PoolFee = currency.PoolFee,
						Summary = currency.Info.Description,
						TradeFee = currency.TradeFee,
						Type = currency.Type,
						TipMin = currency.MinTip,
						WithdrawFee = currency.WithdrawFee,
						WithdrawFeeType = currency.WithdrawFeeType,
						WithdrawMax = currency.MaxWithdraw,
						WithdrawMin = currency.MinWithdraw,
						Website = currency.Info.Website,
						Rank = currency.Rank,

						QrFormat = currency.Settings.QrFormat,
						DepositInstructions = currency.Settings.DepositInstructions,
						DepositMessage = currency.Settings.DepositMessage,
						DepositMessageType = currency.Settings.DepositMessageType,
						WithdrawInstructions = currency.Settings.WithdrawInstructions,
						WithdrawMessage = currency.Settings.WithdrawMessage,
						WithdrawMessageType = currency.Settings.WithdrawMessageType,
						AddressType = currency.Settings.AddressType
					}).OrderBy(c => c.Name)
					.ToListNoLockAsync().ConfigureAwait(false);
				return summary;
			}
		}
	}
}