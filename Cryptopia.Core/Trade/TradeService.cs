using Cryptopia.Base;
using Cryptopia.Common.Balance;
using Cryptopia.Common.Cache;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.Exchange;
using Cryptopia.Common.Notifications;
using Cryptopia.Common.Trade;
using Cryptopia.Common.TradeNotification;
using Cryptopia.Common.TradePair;
using Cryptopia.Common.UserVerification;
using Cryptopia.Common.Withdraw;
using Cryptopia.Core.TradeService;
using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptopia.Core.Trade
{
	public class TradeService : ITradeService
	{
		private static readonly string TradeServiceUsername = ConfigurationManager.AppSettings["TradeServiceUsername"];
		private static readonly string TradeServicePassword = ConfigurationManager.AppSettings["TradeServicePassword"];
		private static readonly string TradeServiceDomain = ConfigurationManager.AppSettings["TradeServiceDomain"];

		public ICacheService CacheService { get; set; }
		public ITradePairReader TradePairReader { get; set; }
		public IExchangeReader ExchangeReader { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }
		public IBalanceEstimationService BalanceEstimationService { get; set; }
		public IUserVerificationReader UserVerificationReader { get; set; }

		public async Task<CreateTradeResponseModel> CreateTrade(string userId, CreateTradeModel model, bool isApi)
		{
			try
			{
				using (var tradeService = CreateService())
				{
					var response = await tradeService.SubmitTradeAsync(new SubmitTradeItemRequest
					{
						Amount = model.Amount,
						IsBuy = model.IsBuy,
						Rate = model.Price,
						UserId = new Guid(userId),
						TradePairId = model.TradePairId,
						IsApi = isApi
					}).ConfigureAwait(false);
					return new CreateTradeResponseModel
					{
						Error = response.Error,
						OrderId = response.OrderId,
						FillerOrders = response.FilledOrderIds,
						DataUpdates = await ProcessDataUpdates(response),
						Notifications = await ProcessNotifications(response)
					};
				}
			}
			catch (Exception)
			{
				return new CreateTradeResponseModel { Error = "An Error occurred placing trade, if problem persist please contact Cryptopia Support" };
			}
		}

		public async Task<CancelTradeResponseModel> CancelTrade(string userId, CancelTradeModel model, bool isApi)
		{
			try
			{
				using (var tradeService = CreateService())
				{
					var response = await tradeService.CancelTradeAsync(new CancelTradeItemRequest
					{
						CancelType = model.CancelType,
						TradeId = model.TradeId,
						TradePairId = model.TradePairId,
						UserId = new Guid(userId),
						IsApi = isApi
					}).ConfigureAwait(false);
					return new CancelTradeResponseModel
					{
						Error = response.Error,
						CanceledOrders = response.CanceledOrderIds,
						DataUpdates = await ProcessDataUpdates(response),
						Notifications = await ProcessNotifications(response),
					};
				}
			}
			catch (Exception)
			{
				return new CancelTradeResponseModel { Error = "An Error occurred canceling trade, if problem persist please contact Cryptopia Support" };
			}
		}

		public async Task<CreateTipResponseModel> CreateTip(string userId, CreateTipModel model, bool isApi)
		{
			try
			{
				var estimatedPrice = await BalanceEstimationService.GetNZDPerCoin(model.CurrencyId).ConfigureAwait(false);
				var estimatedTotal = model.Amount * estimatedPrice;
				var verificationResult = await UserVerificationReader.GetVerificationStatus(userId);
				if (verificationResult.Level != VerificationLevel.Legacy && (verificationResult.Current + estimatedTotal) > verificationResult.Limit)
					return new CreateTipResponseModel { Error = $"Tip exceeds daily limit of ${verificationResult.Limit:F2} NZD." };

				using (var tradeService = CreateService())
				{
					var response = await tradeService.SubmitTipAsync(new SubmitTipRequest
					{
						Amount = model.Amount,
						CurrencyId = model.CurrencyId,
						UserId = new Guid(userId),
						UserTo = model.UserTo,
						EstimatedPrice = estimatedPrice,
						IsApi = isApi
					}).ConfigureAwait(false);
					return new CreateTipResponseModel
					{
						Error = response.Error,
						Notifications = await ProcessTipNotifications(response)
					};
				}
			}
			catch (Exception)
			{
				return new CreateTipResponseModel { Error = "An Error occurred placing tip, if problem persist please contact Cryptopia Support" };
			}
		}

		public async Task<CreateTransferResponseModel> CreateTransfer(string userId, CreateTransferModel model, bool isApi)
		{
			try
			{
				var estimatedPrice = await BalanceEstimationService.GetNZDPerCoin(model.CurrencyId).ConfigureAwait(false);
				var estimatedTotal = model.Amount * estimatedPrice;
				var verificationResult = await UserVerificationReader.GetVerificationStatus(userId);
				if (verificationResult.Level != VerificationLevel.Legacy && model.TransferType != TransferType.Paytopia && (verificationResult.Current + estimatedTotal) > verificationResult.Limit)
					return new CreateTransferResponseModel { Error = $"Transfer exceeds daily limit of ${verificationResult.Limit:F2} NZD." };

				using (var tradeService = CreateService())
				{
					var response = await tradeService.SubmitTransferAsync(new SubmitTransferRequest
					{
						Amount = model.Amount,
						CurrencyId = model.CurrencyId,
						UserId = new Guid(userId),
						UserTo = new Guid(model.Receiver),
						TransferType = model.TransferType,
						EstimatedPrice = estimatedTotal,
						IsApi = isApi
					}).ConfigureAwait(false);
					return new CreateTransferResponseModel
					{
						Error = response.Error,
						TransferId = response.TransferId,
						Notifications = await ProcessTransferNotifications(response),
					};
				}
			}
			catch (Exception)
			{
				return new CreateTransferResponseModel { Error = "An Error occurred placing transfer, if problem persist please contact Cryptopia Support" };
			}
		}

		public async Task<CreateLottoResponseModel> CreateLotto(string userId, CreateLottoModel model, bool isApi)
		{
			try
			{
				using (var tradeService = CreateService())
				{
					var response = await tradeService.SubmitLottoAsync(new SubmitLottoRequest
					{
						EntryCount = model.EntryCount,
						LottoItemId = model.LottoItemId,
						UserId = new Guid(userId),
						IsApi = isApi
					}).ConfigureAwait(false);
					return new CreateLottoResponseModel
					{
						Error = response.Error,
						//	Notifications = await ProcessNotifications(response),
					};
				}
			}
			catch (Exception)
			{
				return new CreateLottoResponseModel { Error = "An Error occurred creating lotto ticket, if problem persist please contact Cryptopia Support" };
			}
		}

		public async Task<CreateWithdrawResponseModel> CreateWithdraw(string userId, CreateWithdrawModel model, bool isApi)
		{
			try
			{
				var estimatedPrice = await BalanceEstimationService.GetNZDPerCoin(model.CurrencyId).ConfigureAwait(false);
				var estimatedTotal = model.Amount * estimatedPrice;
				var verificationResult = await UserVerificationReader.GetVerificationStatus(userId);
				if (verificationResult.Level != VerificationLevel.Legacy && (verificationResult.Current + estimatedTotal) > verificationResult.Limit)
					return new CreateWithdrawResponseModel { Error = $"Withdraw exceeds daily limit of ${verificationResult.Limit:F2} NZD." };

				using (var tradeService = CreateService())
				{
					var response = await tradeService.SubmitWithdrawAsync(new SubmitWithdrawRequest
					{
						Address = model.Address,
						Amount = model.Amount,
						CurrencyId = model.CurrencyId,
						TwoFactorToken = model.TwoFactorToken,
						Type = model.Type,
						UserId = new Guid(userId),
						EstimatedPrice = estimatedTotal,
						IsApi = isApi
					}).ConfigureAwait(false);
					return new CreateWithdrawResponseModel
					{
						Error = response.Error,
						WithdrawId = response.WithdrawId
						//	Notifications = await ProcessNotifications(response),
					};
				}
			}
			catch (Exception)
			{
				return new CreateWithdrawResponseModel { Error = "An Error occurred creating withdraw request, if problem persist please contact Cryptopia Support" };
			}
		}


		public TradeProcessorClient CreateService()
		{
			var client = new TradeProcessorClient();
#if !DEBUG
			client.ClientCredentials.Windows.ClientCredential.UserName = TradeServiceUsername;
			client.ClientCredentials.Windows.ClientCredential.Password = TradeServicePassword;
			client.ClientCredentials.Windows.ClientCredential.Domain = TradeServiceDomain;
#endif
			return client;
		}

		private async Task<List<INotification>> ProcessNotifications(CancelTradeItemResponse response)
		{
			var results = new List<INotification>();
			if (!response.CanceledOrders.IsNullOrEmpty())
			{
				foreach (var canceledOrder in response.CanceledOrders)
				{
					results.Add(new NotificationModel
					{
						Type = NotificationLevelType.Info,
						Header = $"Canceled Order",
						Notification = $"Canceled {canceledOrder.Type} Order #{canceledOrder.Id}, {canceledOrder.Remaining:F8} @ {canceledOrder.Rate:F8}",
						UserId = canceledOrder.UserId
					});
				}

				using (var context = DataContextFactory.CreateContext())
				{
					if (!results.IsNullOrEmpty())
					{
						foreach (var notification in results)
						{
							context.Notifications.Add(new Entity.UserNotification
							{
								Title = notification.Header,
								Notification = notification.Notification,
								Type = notification.Type.ToString(),
								UserId = notification.UserId.ToString(),
								Timestamp = DateTime.UtcNow
							});
						}
					}
					await context.SaveChangesAsync().ConfigureAwait(false);
				}
			}

			return results;
		}

		private async Task<List<INotification>> ProcessNotifications(SubmitTradeItemResponse response)
		{
			var results = new List<INotification>();
			if (!response.FilledOrders.IsNullOrEmpty())
			{
				foreach (var filledOrder in response.FilledOrders)
				{
					results.Add(new NotificationModel
					{
						Type = NotificationLevelType.Info,
						Header = $"Order Filled",
						Notification = $"You sold {filledOrder.Amount:F8} {filledOrder.Symbol} for {filledOrder.Amount * filledOrder.Rate:F8} {filledOrder.BaseSymbol}",
						UserId = filledOrder.ToUserId
					});

					results.Add(new NotificationModel
					{
						Type = NotificationLevelType.Info,
						Header = $"Order Filled",
						Notification = $"You bought {filledOrder.Amount:F8} {filledOrder.Symbol} for {filledOrder.Amount * filledOrder.Rate:F8} {filledOrder.BaseSymbol}",
						UserId = filledOrder.UserId
					});
				}
			}

			if (response.Order != null)
			{
				results.Add(new NotificationModel
				{
					Type = NotificationLevelType.Info,
					Header = $"Order Placed",
					Notification = $"Placed {response.Order.Type} order.{Environment.NewLine}{response.Order.Amount:F8} {response.Order.Symbol} @ {response.Order.Rate:F8} {response.Order.BaseSymbol}",
					UserId = response.Order.UserId
				});
			}

			if (response.OrderRefund != null)
			{
				results.Add(new NotificationModel
				{
					Type = NotificationLevelType.Info,
					Header = $"Order Refund",
					Notification = $"Refunded {response.OrderRefund.Amount:F8} {response.OrderRefund.Symbol} from buy order.",
					UserId = response.OrderRefund.UserId
				});
			}

			if (!results.IsNullOrEmpty())
			{
				using (var context = DataContextFactory.CreateContext())
				{
					foreach (var notification in results)
					{
						context.Notifications.Add(new Entity.UserNotification
						{
							Title = notification.Header,
							Notification = notification.Notification,
							Type = notification.Type.ToString(),
							UserId = notification.UserId.ToString(),
							Timestamp = DateTime.UtcNow
						});
					}
					await context.SaveChangesAsync().ConfigureAwait(false);
				}
			}
			return results;
		}

		private async Task<List<ITradeDataUpdate>> ProcessDataUpdates(SubmitTradeItemResponse response)
		{
			var cacheUsers = new HashSet<Guid>();
			var cacheTradePairs = new HashSet<int>();
			var results = new List<ITradeDataUpdate>();
			if (!response.FilledOrders.IsNullOrEmpty())
			{
				foreach (var order in response.FilledOrders)
				{
					results.Add(new TradeOrderBookUpdate
					{
						Amount = order.Amount,
						Rate = order.Rate,
						Total = order.Amount * order.Rate,
						Type = order.Type,
						TradePairId = order.TradePairId,
						Action = TradeUpdateAction.Remove
					});

					results.Add(new TradeHistoryUpdate
					{
						Amount = order.Amount,
						Rate = order.Rate,
						Total = order.Amount * order.Rate,
						Timestamp = order.Timestamp,
						Type = order.Type,
						TradePairId = order.TradePairId
					});

					results.Add(new TradeUserHistoryUpdate
					{
						UserId = order.UserId,
						Amount = order.Amount,
						Rate = order.Rate,
						Total = order.Amount * order.Rate,
						Timestamp = order.Timestamp,
						Type = TradeHistoryType.Buy,
						TradePairId = order.TradePairId
					});

					results.Add(new TradeUserHistoryUpdate
					{
						UserId = order.ToUserId,
						Amount = order.Amount,
						Rate = order.Rate,
						Total = order.Amount * order.Rate,
						Timestamp = order.Timestamp,
						Type = TradeHistoryType.Sell,
						TradePairId = order.TradePairId
					});

					cacheUsers.Add(order.UserId);
					cacheUsers.Add(order.ToUserId);
					cacheTradePairs.Add(order.TradePairId);
				}
			}

			if (response.Order != null)
			{
				results.Add(new TradeOpenOrderUpdate
				{
					OrderId = response.Order.Id,
					UserId = response.Order.UserId,
					Amount = response.Order.Amount,
					Rate = response.Order.Rate,
					Total = response.Order.Amount * response.Order.Rate,
					Type = response.Order.Type,
					TradePairId = response.Order.TradePairId,
					Remaining = response.Order.Remaining,
					Status = response.Order.Status,
					Action = TradeUpdateAction.Add,
					Market = $"{response.Order.Symbol}/{response.Order.BaseSymbol}"
				});

				results.Add(new TradeOrderBookUpdate
				{
					Amount = response.Order.Amount,
					Rate = response.Order.Rate,
					Total = response.Order.Amount * response.Order.Rate,
					Type = response.Order.Type,
					TradePairId = response.Order.TradePairId,
					Action = TradeUpdateAction.Add
				});

				cacheUsers.Add(response.Order.UserId);
				cacheTradePairs.Add(response.Order.TradePairId);
			}

			if (!response.OrdersUpdated.IsNullOrEmpty())
			{
				foreach (var order in response.OrdersUpdated)
				{
					results.Add(new TradeOpenOrderUpdate
					{
						OrderId = order.OrderId,
						UserId = order.UserId,
						Amount = order.Amount,
						Rate = order.Rate,
						Total = order.Amount * order.Rate,
						Type = order.Type,
						TradePairId = order.TradePairId,
						Remaining = order.Remaining,
						Status = order.Remaining != order.Amount
							? TradeStatus.Partial
							: TradeStatus.Pending,
						Action = order.Remaining <= 0
							? TradeUpdateAction.Remove
							: TradeUpdateAction.Update
					});

					cacheUsers.Add(order.UserId);
					cacheTradePairs.Add(order.TradePairId);
				}
			}

			if (response.Ticker != null)
			{
				var priceUpdate = new TradePriceUpdate
				{
					TradePairId = response.Ticker.TradePairId,
					BaseVolume = response.Ticker.BaseVolume,
					Change = response.Ticker.Change,
					High = response.Ticker.High,
					Last = response.Ticker.Last,
					Low = response.Ticker.Low,
					Market = response.Ticker.Market,
					Volume = response.Ticker.Volume,
				};
				results.Add(priceUpdate);
				//await TradePairReader.UpdatePriceCache(priceUpdate).ConfigureAwait(false);
				//await ExchangeReader.UpdatePriceCache(priceUpdate).ConfigureAwait(false);
			}

			// Clear caches
			if (cacheTradePairs.Any())
			{
				foreach (var tradepair in cacheTradePairs)
				{
					await ClearCache(tradepair);
					if (cacheUsers.Any())
					{
						foreach (var user in cacheUsers)
						{
							await ClearCache(tradepair, user);
							results.Add(new TradeBalanceUpdate
							{
								TradePairId = tradepair,
								UserId = user
							});
						}
					}
				}
			}

			return results;
		}

		private async Task<List<ITradeDataUpdate>> ProcessDataUpdates(CancelTradeItemResponse response)
		{
			var cacheUsers = new HashSet<Guid>();
			var cacheTradePairs = new HashSet<int>();
			var results = new List<ITradeDataUpdate>();
			if (!response.CanceledOrders.IsNullOrEmpty())
			{
				foreach (var order in response.CanceledOrders)
				{
					results.Add(new TradeOrderBookUpdate
					{
						Amount = order.Amount,
						Rate = order.Rate,
						Total = order.Amount * order.Rate,
						Type = order.Type,
						TradePairId = order.TradePairId,
						Action = TradeUpdateAction.Cancel
					});

					results.Add(new TradeOpenOrderUpdate
					{
						OrderId = order.Id,
						UserId = order.UserId,
						Amount = order.Amount,
						Rate = order.Rate,
						Total = order.Amount * order.Rate,
						Type = order.Type,
						TradePairId = order.TradePairId,
						Remaining = order.Remaining,
						Status = order.Status,
						Action = TradeUpdateAction.Cancel
					});

					cacheUsers.Add(order.UserId);
					cacheTradePairs.Add(order.TradePairId);
				}
			}

			// Clear caches
			if (cacheTradePairs.Any())
			{
				foreach (var tradepair in cacheTradePairs)
				{
					await ClearCache(tradepair);
					if (cacheUsers.Any())
					{
						foreach (var user in cacheUsers)
						{
							await ClearCache(tradepair, user);
							results.Add(new TradeBalanceUpdate
							{
								TradePairId = tradepair,
								UserId = user
							});
						}
					}
				}
			}
			return results;
		}

		private async Task<List<INotification>> ProcessTipNotifications(SubmitTipResponse response)
		{
			var results = new List<INotification>();
			if (response.Notifications.IsNullOrEmpty())
				return results;

			using (var context = DataContextFactory.CreateContext())
			{
				foreach (var notification in response.Notifications)
				{
					results.Add(new TipNotificationModel
					{
						Header = "Tip Sent",
						Type = NotificationLevelType.Info,
						UserId = notification.Sender,
						Receivers = notification.Receivers,
						Notification = notification.SenderMessage,
						ReceiverMessage = notification.ReceiverMessage
					});

					if (!string.IsNullOrEmpty(notification.SenderMessage))
					{
						context.Notifications.Add(new Entity.UserNotification
						{
							Title = "Tip Sent",
							Notification = notification.SenderMessage,
							Type = "Info",
							UserId = notification.Sender.ToString(),
							Timestamp = DateTime.UtcNow
						});
					}

					if (notification.Receivers.IsNullOrEmpty())
						continue;

					foreach (var user in notification.Receivers)
					{
						context.Notifications.Add(new Entity.UserNotification
						{
							Title = "Tip Received",
							Notification = notification.ReceiverMessage,
							Type = "Info",
							UserId = user.ToString(),
							Timestamp = DateTime.UtcNow
						});
					}
				}
				await context.SaveChangesAsync().ConfigureAwait(false);
			}
			return results;
		}

		private async Task<List<INotification>> ProcessTransferNotifications(SubmitTransferResponse response)
		{
			var results = new List<INotification>();
			if (response.Notifications.IsNullOrEmpty())
				return results;

			using (var context = DataContextFactory.CreateContext())
			{
				foreach (var notification in response.Notifications)
				{
					context.Notifications.Add(new Entity.UserNotification
					{
						Title = notification.Header,
						Notification = notification.Notification,
						Type = notification.Type.ToString(),
						UserId = notification.UserId.ToString(),
						Timestamp = DateTime.UtcNow
					});

					results.Add(new NotificationModel
					{
						Header = notification.Header,
						Type = notification.Type,
						UserId = notification.UserId,
						Notification = notification.Notification
					});
				}
				await context.SaveChangesAsync().ConfigureAwait(false);
			}
			return results;
		}


		private async Task ClearCache(int tradePairId)
		{
			await CacheService.InvalidateAsync
			(
				CacheKey.ExchangeTradeHistory(tradePairId),
				CacheKey.ExchangeOrderBook(tradePairId),
				//CacheKey.ExchangeStockChart(tradePairId, 30),
				CacheKey.ExchangeDepthChart(tradePairId),
				CacheKey.ApiMarketHistory(tradePairId, 24)
			);
		}

		private async Task ClearCache(int tradePairId, Guid userId)
		{
			await CacheService.InvalidateAsync
			(
				CacheKey.ExchangeUserOpenOrders(userId.ToString(), tradePairId),
				CacheKey.ExchangeUserTradeHistory(userId.ToString(), tradePairId),
				CacheKey.ExchangeUserOpenOrderDataTable(userId.ToString()),
				CacheKey.ExchangeUserOrderBook(userId.ToString(), tradePairId)
			);
		}
	}
}
