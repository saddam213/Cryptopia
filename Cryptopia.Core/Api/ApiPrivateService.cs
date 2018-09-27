using Cryptopia.Base;
using Cryptopia.Common.Address;
using Cryptopia.Common.Api;
using Cryptopia.Common.Cache;
using Cryptopia.Common.Currency;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.Deposit;
using Cryptopia.Common.Trade;
using Cryptopia.Common.TradePair;
using Cryptopia.Common.User;
using Cryptopia.Common.UserVerification;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.DataContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptopia.Core.Api
{
	public class ApiPrivateService : IApiPrivateService
	{
		public ICacheService CacheService { get; set; }
		public ITradeService TradeService { get; set; }
		public IAddressReader AddressReader { get; set; }
		public ITradePairReader TradePairReader { get; set; }
		public IUserBalanceReader UserBalanceReader { get; set; }
		public ICurrencyReader CurrencyReader { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }
		public IUserReader UserReader { get; set; }
		public IUserVerificationReader UserVerificationReader { get; set; }
		public IDepositService DepositService { get; set; }


		public async Task<ApiUserBalanceResponse> GetUserBalance(ApiUserBalanceRequest request)
		{
			if (request.CurrencyId.HasValue || !string.IsNullOrEmpty(request.Currency))
			{
				var currency = request.CurrencyId.HasValue
					? await CurrencyReader.GetCurrency(request.CurrencyId.Value).ConfigureAwait(false)
					: await CurrencyReader.GetCurrency(request.Currency).ConfigureAwait(false);
				if (currency == null)
					return new ApiUserBalanceResponse { Success = false, Error = "Currency not found." };

				var balance = await UserBalanceReader.GetBalance(request.UserId.ToString(), currency.CurrencyId).ConfigureAwait(false);
				if (balance == null)
					return new ApiUserBalanceResponse { Success = false, Error = "No balance found" };

				var balanceItem = new ApiBalance
				{
					CurrencyId = balance.CurrencyId,
					Symbol = balance.Symbol,
					Available = balance.Available,
					Unconfirmed = balance.Unconfirmed,
					HeldForTrades = balance.HeldForTrades,
					PendingWithdraw = balance.PendingWithdraw,
					Total = balance.Total,
					Status = balance.Status.ToString(),
					StatusMessage = balance.StatusMessage,
					Address = balance.Address,
					BaseAddress = balance.BaseAddress
				};

				return new ApiUserBalanceResponse
				{
					Success = true,
					Data = new List<ApiBalance> { balanceItem }
				};
			}
			else
			{
				var balances = await UserBalanceReader.GetBalances(request.UserId.ToString(), false).ConfigureAwait(false);
				if (balances.Balances.IsNullOrEmpty())
					return new ApiUserBalanceResponse { Success = false, Error = "No balance found" };

				var balanceItems = balances.Balances.Select(balance => new ApiBalance
				{
					CurrencyId = balance.CurrencyId,
					Symbol = balance.Symbol,
					Available = balance.Available,
					Unconfirmed = balance.Unconfirmed,
					HeldForTrades = balance.HeldForTrades,
					PendingWithdraw = balance.PendingWithdraw,
					Total = balance.Total,
					Status = balance.Status.ToString(),
					StatusMessage = balance.StatusMessage,
					Address = balance.Address,
					BaseAddress = balance.BaseAddress
				});

				return new ApiUserBalanceResponse
				{
					Success = true,
					Data = new List<ApiBalance>(balanceItems)
				};
			}
		}

		public async Task<ApiUserDepositAddressResponse> GetUserDepositAddress(ApiUserDepositAddressRequest request)
		{
			var currency = request.CurrencyId.HasValue
					? await CurrencyReader.GetCurrency(request.CurrencyId.Value).ConfigureAwait(false)
					: await CurrencyReader.GetCurrency(request.Currency).ConfigureAwait(false);
			if (currency == null)
				return new ApiUserDepositAddressResponse { Success = false, Error = "Currency not found." };

			var addressResponse = await AddressReader.GetAddress(request.UserId.ToString(), currency.CurrencyId).ConfigureAwait(false);
			if (addressResponse == null || !string.IsNullOrEmpty(addressResponse.ErrorMessage))
				return new ApiUserDepositAddressResponse { Success = false, Error = addressResponse.ErrorMessage ?? "Currency not found." };

			return new ApiUserDepositAddressResponse
			{
				Success = true,
				Data = new ApiDepositAddressData
				{
					Currency = currency.Symbol,
					Address = addressResponse.AddressData,
					BaseAddress = currency.BaseAddress
				}
			};
		}

		public async Task<ApiUserOpenOrdersResponse> GetUserOpenOrders(ApiUserOpenOrdersRequest request)
		{
			if (request.TradePairId.HasValue || !string.IsNullOrEmpty(request.Market))
			{
				var tradepair = request.TradePairId.HasValue
					? await TradePairReader.GetTradePair(request.TradePairId.Value, true).ConfigureAwait(false)
					: await TradePairReader.GetTradePair(request.Market.Replace('/', '_'), true).ConfigureAwait(false);
				if (tradepair == null)
					return new ApiUserOpenOrdersResponse { Success = false, Error = "Market not found." };

				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var openOrders = context.Trade
					.AsNoTracking()
					.Where(x => x.UserId == request.UserId && x.TradePairId == tradepair.TradePairId && (x.Status == TradeStatus.Partial || x.Status == TradeStatus.Pending))
					.OrderByDescending(x => x.Id)
					.Select(x => new ApiOpenOrder
					{
						Amount = x.Amount,
						Market = string.Concat(x.TradePair.Currency1.Symbol, "/", x.TradePair.Currency2.Symbol),
						OrderId = x.Id,
						Rate = x.Rate,
						Remaining = x.Remaining,
						TimeStamp = x.Timestamp,
						TradePairId = x.TradePairId,
						Type = x.Type.ToString()
					});
					return new ApiUserOpenOrdersResponse
					{
						Success = true,
						Data = await openOrders.ToListNoLockAsync().ConfigureAwait(false)
					};
				}
			}
			else
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var openOrders = context.Trade
					.AsNoTracking()
					.Where(x => x.UserId == request.UserId && (x.Status == TradeStatus.Partial || x.Status == TradeStatus.Pending))
					.OrderByDescending(x => x.Id)
					.Select(x => new ApiOpenOrder
					{
						Amount = x.Amount,
						Market = string.Concat(x.TradePair.Currency1.Symbol, "/", x.TradePair.Currency2.Symbol),
						OrderId = x.Id,
						Rate = x.Rate,
						Remaining = x.Remaining,
						TimeStamp = x.Timestamp,
						TradePairId = x.TradePairId,
						Type = x.Type.ToString()
					});

					return new ApiUserOpenOrdersResponse
					{
						Success = true,
						Data = await openOrders.ToListNoLockAsync().ConfigureAwait(false)
					};
				}
			}
		}

		public async Task<ApiUserTradeHistoryResponse> GetUserTradeHistory(ApiUserTradeHistoryRequest request)
		{
			var count = Math.Min(request.Count ?? 100, 1000);
			if (request.TradePairId.HasValue || !string.IsNullOrEmpty(request.Market))
			{
				var tradepair = request.TradePairId.HasValue
					? await TradePairReader.GetTradePair(request.TradePairId.Value, true).ConfigureAwait(false)
					: await TradePairReader.GetTradePair(request.Market.Replace('/', '_'), true).ConfigureAwait(false);
				if (tradepair == null)
					return new ApiUserTradeHistoryResponse { Success = false, Error = "Market not found." };

				var cacheResult = await CacheService.GetOrSetMemoryAsync(CacheKey.ApiUserTradeHistory(request.UserId.ToString(), tradepair.TradePairId), TimeSpan.FromSeconds(1), async () =>
				{
					using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
					{
						var history = context.TradeHistory
							.AsNoTracking()
							.Where(x => x.TradePairId == tradepair.TradePairId && (x.UserId == request.UserId || x.ToUserId == request.UserId))
							.OrderByDescending(x => x.Id)
							.Take(1000)
							.Select(x => new ApiTradeHistory
							{
								Market = string.Concat(x.TradePair.Currency1.Symbol, "/", x.TradePair.Currency2.Symbol),
								TradeId = x.Id,
								Amount = x.Amount,
								Rate = x.Rate,
								Type = x.UserId == request.UserId
									? TradeHistoryType.Buy.ToString()
									: TradeHistoryType.Sell.ToString(),
								TimeStamp = x.Timestamp,
								TradePairId = x.TradePairId,
								Fee = x.Fee
							});
						return await history.ToListNoLockAsync().ConfigureAwait(false);
					}
				}).ConfigureAwait(false);

				return new ApiUserTradeHistoryResponse
				{
					Success = true,
					Data = cacheResult.Take(count).ToList()
				};
			}
			else
			{
				var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ApiUserTradeHistory(request.UserId.ToString()), TimeSpan.FromSeconds(20), async () =>
				{
					using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
					{
						var history = context.TradeHistory
							.AsNoTracking()
							.Where(x => x.UserId == request.UserId || x.ToUserId == request.UserId)
							.OrderByDescending(x => x.Id)
							.Take(1000)
							.Select(x => new ApiTradeHistory
							{
								Market = string.Concat(x.TradePair.Currency1.Symbol, "/", x.TradePair.Currency2.Symbol),
								TradeId = x.Id,
								Amount = x.Amount,
								Rate = x.Rate,
								Type = x.UserId == request.UserId
									? TradeHistoryType.Buy.ToString()
									: TradeHistoryType.Sell.ToString(),
								TimeStamp = x.Timestamp,
								TradePairId = x.TradePairId,
								Fee = x.Fee
							});

						return await history.ToListNoLockAsync().ConfigureAwait(false);
					}
				}).ConfigureAwait(false);

				return new ApiUserTradeHistoryResponse
				{
					Success = true,
					Data = cacheResult.Take(count).ToList()
				};
			}

		}

		public async Task<ApiUserTransactionsResponse> GetUserTransactions(ApiUserTransactionsRequest request)
		{
			var count = Math.Min(request.Count ?? 100, 1000);
			if(request.Type == TransactionType.All)
			{
				var deposits = await GetDeposits(request);
				var withdrawals = await GetWithdrawals(request);
				var allTransactions = deposits.Concat(withdrawals).OrderByDescending(x => x.Timestamp);
				return new ApiUserTransactionsResponse
				{
					Success = true,
					Data = allTransactions.Take(count).ToList()
				};
			}
			else if (request.Type == TransactionType.Deposit)
			{
				var deposits = await GetDeposits(request);
				return new ApiUserTransactionsResponse
				{
					Success = true,
					Data = deposits.Take(count).ToList()
				};
			}
			else
			{
				var withdrawals = await GetWithdrawals(request);
				return new ApiUserTransactionsResponse
				{
					Success = true,
					Data = withdrawals.Take(count).ToList()
				};
			}
		}

		public async Task<CreateTradeResponseModel> SubmitUserTrade(ApiSubmitUserTradeRequest request)
		{
			var tradePair = request.TradePairId.HasValue
				? await TradePairReader.GetTradePair(request.TradePairId.Value).ConfigureAwait(false)
				: await TradePairReader.GetTradePair(request.Market.Replace('/', '_')).ConfigureAwait(false);
			if (tradePair == null)
			{
				var marketName = request.TradePairId.HasValue ? request.TradePairId.ToString() : request.Market;
				return new CreateTradeResponseModel { Error = $"Market '{marketName}' not found." };
			}

			var totalTrade = request.Amount * request.Rate;
			if (totalTrade < tradePair.BaseMinTrade)
				return new CreateTradeResponseModel { Error = $"Invalid trade amount, Minimum total trade is {tradePair.BaseMinTrade} {tradePair.BaseSymbol}" };

			var response = await TradeService.CreateTrade(request.UserId.ToString(), new CreateTradeModel
			{
				Amount = request.Amount,
				Price = request.Rate,
				TradePairId = tradePair.TradePairId,
				IsBuy = request.Type == TradeHistoryType.Buy,
			}, true).ConfigureAwait(false);

			return response;
		}

		public async Task<CancelTradeResponseModel> CancelUserTrade(ApiCancelUserTradeRequest request)
		{
			var response = await TradeService.CancelTrade(request.UserId.ToString(), new CancelTradeModel
			{
				CancelType = request.CancelType,
				TradeId = request.TradeId ?? 0,
				TradePairId = request.TradePairId ?? 0,
			}, true).ConfigureAwait(false);
			return response;
		}

		public async Task<CreateTipResponseModel> SubmitUserTip(ApiSubmitUserTipRequest request)
		{
			var currency = request.CurrencyId.HasValue
				? await CurrencyReader.GetCurrency(request.CurrencyId.Value).ConfigureAwait(false)
				: await CurrencyReader.GetCurrency(request.Currency).ConfigureAwait(false);
			if (currency == null)
				return new CreateTipResponseModel { Error = "Currency not found." };

			var now = DateTime.UtcNow;
			if (currency.TippingExpires < now)
				return new CreateTipResponseModel { Error = $"Tipping is not currently enabled for {currency.Symbol}." };

			if (request.ActiveUsers < 2)
				return new CreateTipResponseModel { Error = "Minimum 'ActiveUsers' is 2" };

			if (request.ActiveUsers > 100)
				return new CreateTipResponseModel { Error = "Maximum 'ActiveUsers' is 100" };

			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var sender = await context.Users
				.AsNoTracking()
				.FirstOrDefaultAsync(x => x.Id == request.UserId.ToString()).ConfigureAwait(false);
				if (sender == null)
					return new CreateTipResponseModel { Error = "Unauthorized." };

				if (sender.ChatTipBanEndTime.HasValue && sender.ChatTipBanEndTime.Value > DateTime.UtcNow)
					return new CreateTipResponseModel { Error = $"You are currently banned from tipping for {(sender.ChatTipBanEndTime.Value - DateTime.UtcNow).TotalSeconds} seconds" };

				var chatBotId = Constant.SYSTEM_USER_CHATBOT.ToString();
				var ignoreList = new List<string> { sender.UserName };
				if (!string.IsNullOrEmpty(sender.ChatTipIgnoreList))
				{
					ignoreList.AddRange(sender.ChatTipIgnoreList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
				}

				var usersToTip = await context.ChatMessages
					.AsNoTracking()
					.Where(x => !x.User.DisableTips && !(x.User.ChatTipBanEndTime.HasValue && x.User.ChatTipBanEndTime.Value > DateTime.UtcNow) && !ignoreList.Contains(x.User.UserName) && x.UserId != chatBotId)
					.OrderByDescending(x => x.Timestamp)
					.Select(x => x.UserId)
					.Distinct()
					.Take(request.ActiveUsers)
					.ToListNoLockAsync().ConfigureAwait(false);
				if (!usersToTip.Any())
					return new CreateTipResponseModel { Error = "No valid users found to tip" };

				if (request.Amount < (currency.TipMin * usersToTip.Count))
					return new CreateTipResponseModel { Error = $"Minimum tip amount is {currency.TipMin:F8} {currency.Symbol} per user." };

				var response = await TradeService.CreateTip(request.UserId.ToString(), new CreateTipModel
				{
					Amount = request.Amount,
					CurrencyId = currency.CurrencyId,
					UserTo = usersToTip.Select(Guid.Parse).ToList()
				}, true).ConfigureAwait(false);
				return response;
			}
		}

		public async Task<CreateTransferResponseModel> SubmitUserTransfer(ApiSubmitUserTransferRequest request)
		{
			var currency = request.CurrencyId.HasValue
				? await CurrencyReader.GetCurrency(request.CurrencyId.Value).ConfigureAwait(false)
				: await CurrencyReader.GetCurrency(request.Currency).ConfigureAwait(false);
			if (currency == null)
				return new CreateTransferResponseModel { Error = "Currency not found." };

			var receiver = await UserReader.GetUserByName(request.UserName).ConfigureAwait(false);
			if (receiver == null)
				return new CreateTransferResponseModel { Error = "Receiver not found." };

			var response = await TradeService.CreateTransfer(request.UserId.ToString(), new CreateTransferModel
			{
				CurrencyId = currency.CurrencyId,
				Amount = request.Amount,
				TransferType = TransferType.User,
				Receiver = receiver.UserId
			}, true).ConfigureAwait(false);

			return response;
		}

		public async Task<ApiSubmitUserWithdrawResponse> SubmitUserWithdraw(ApiSubmitUserWithdrawRequest request)
		{
			var currency = request.CurrencyId.HasValue
					? await CurrencyReader.GetCurrency(request.CurrencyId.Value).ConfigureAwait(false)
					: await CurrencyReader.GetCurrency(request.Currency).ConfigureAwait(false);
			if (currency == null)
				return new ApiSubmitUserWithdrawResponse { Success = false, Error = "Currency not found." };

			if (currency.Status == CurrencyStatus.Maintenance || currency.Status == CurrencyStatus.Offline || currency.Status == CurrencyStatus.NoConnections)
				return new ApiSubmitUserWithdrawResponse { Success = false, Error = $"Currency is currently not available for withdraw, Status: {currency.Status}" };

			if (request.Amount < currency.WithdrawMin)
				return new ApiSubmitUserWithdrawResponse { Success = false, Error = $"Withdraw amount is below the minimum, Minimum: {currency.WithdrawMin:F8} {currency.Symbol}" };

			if (request.Amount > currency.WithdrawMax)
				return new ApiSubmitUserWithdrawResponse { Success = false, Error = $"Withdraw amount is above the maximum, Maximum: {currency.WithdrawMax:F8} {currency.Symbol}" };

			var balance = await UserBalanceReader.GetBalance(request.UserId.ToString(), currency.CurrencyId).ConfigureAwait(false);
			if (balance == null || request.Amount > balance.Available)
				return new ApiSubmitUserWithdrawResponse { Success = false, Error = "Insufficient funds." };

			var user = await UserReader.GetUserById(request.UserId.ToString()).ConfigureAwait(false);
			if (user == null || !user.IsApiWithdrawEnabled)
				return new ApiSubmitUserWithdrawResponse { Success = false, Error = "Your API withdraw setting is currently disabled." };

			var address = request.Address;
			if (currency.AddressType != AddressType.Standard)
				address = $"{request.Address}:{request.PaymentId ?? string.Empty}";
			if (currency.Type == CurrencyType.Fiat)
				address = address.TrimEnd(':');

			if (!user.IsApiUnsafeWithdrawEnabled)
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var validAddress = await context.AddressBook
						.AsNoTracking()
						.FirstOrDefaultAsync(x => x.UserId == request.UserId && x.CurrencyId == currency.CurrencyId && x.Address == address && x.IsEnabled).ConfigureAwait(false);
					if (validAddress == null)
						return new ApiSubmitUserWithdrawResponse { Success = false, Error = $"Address does not exist in your secure Withdraw Address Book" };
				}
			}
			else
			{
				if (currency.CurrencyId == Constant.NZDT_ID && !UserVerificationReader.IsVerified(user.VerificationLevel))
				{
					return new ApiSubmitUserWithdrawResponse { Success = false, Error = $"Id verification required for NZDT services." };
				}

				if(!await DepositService.ValidateAddress(currency.CurrencyId, address))
					return new ApiSubmitUserWithdrawResponse { Success = false, Error = $"Invalid {currency.Symbol} address." };
			}

			var response = await TradeService.CreateWithdraw(request.UserId.ToString(), new CreateWithdrawModel
			{
				Address = address,
				Amount = Math.Max(0, request.Amount),
				CurrencyId = balance.CurrencyId,
				TwoFactorToken = string.Empty,
				Type = WithdrawType.Normal
			}, true).ConfigureAwait(false);

			if (response.IsError)
				return new ApiSubmitUserWithdrawResponse { Success = false, Error = response.Error };

			return new ApiSubmitUserWithdrawResponse
			{
				Success = true,
				Data = response.WithdrawId
			};
		}

		private async Task<List<ApiTransactionResult>> GetDeposits(ApiUserTransactionsRequest request)
		{

			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ApiUserTransactions(request.UserId.ToString(), TransactionType.Deposit), TimeSpan.FromSeconds(60), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					return await context.Deposit
					.AsNoTracking()
					.Where(x => x.UserId == request.UserId && x.Status != DepositStatus.Invalid)
					.OrderByDescending(x => x.Id)
					.Take(1000)
					.Select(x => new ApiTransactionResult
					{
						Amount = x.Amount,
						Confirmations = x.Confirmations,
						Currency = x.Currency.Symbol,
						Fee = 0,
						Id = x.Id,
						Status = x.Status.ToString(),
						Timestamp = x.TimeStamp,
						TxId = x.Txid,
						Type = "Deposit"
					}).ToListNoLockAsync().ConfigureAwait(false);
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		private async Task<List<ApiTransactionResult>> GetWithdrawals(ApiUserTransactionsRequest request)
		{
			var count = Math.Min(request.Count ?? 100, 1000);
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.ApiUserTransactions(request.UserId.ToString(), TransactionType.Withdraw), TimeSpan.FromSeconds(60), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					return await context.Withdraw
						.AsNoTracking()
						.Where(x => x.UserId == request.UserId)
						.OrderByDescending(x => x.Id)
						.Take(1000)
						.Select(x => new ApiTransactionResult
						{
							Amount = x.Amount,
							Confirmations = x.Confirmations,
							Currency = x.Currency.Symbol,
							Fee = 0,
							Id = x.Id,
							Status = x.Status.ToString(),
							Timestamp = x.TimeStamp,
							TxId = x.Txid,
							Type = "Withdraw",
							Address = x.Address
						}).ToListNoLockAsync().ConfigureAwait(false);
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

	}
}
