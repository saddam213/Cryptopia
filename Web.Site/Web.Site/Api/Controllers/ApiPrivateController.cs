using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Web.Site.Api.Authentication;
using Web.Site.Api.Models;
using Web.Site.Notifications;
using WebApiThrottle;
using Cryptopia.Enums;
using Cryptopia.Common.Api;
using Cryptopia.Common.Trade;

namespace Web.Site.Api.Controllers
{
	[AllowAnonymous]
	[ApiAuthentication]
	public class ApiPrivateController : ApiController
	{
		public IApiPrivateService ApiPrivateService { get; set; }

		[HttpPost]
		public async Task<ApiSubmitUserTradeResponse> SubmitTrade(ApiSubmitTrade request)
		{
			try
			{
				var response = await ApiPrivateService.SubmitUserTrade(new ApiSubmitUserTradeRequest
				{
					Amount = request.Amount,
					Market = request.Market,
					Rate = request.Rate,
					TradePairId = request.TradePairId,
					Type = request.Type,
					UserId = new Guid(User.Identity.Name),
				});

				if (response.IsError)
					return new ApiSubmitUserTradeResponse { Success = false, Error = response.Error };

				await Task.WhenAll(response.Notifications.SendNotifications(), response.DataUpdates.SendTradeNotifications());

				return new ApiSubmitUserTradeResponse
				{
					Success = true,
					Data = new ApiSubmitUserTradeData
					{
						OrderId = response.OrderId,
						FilledOrders = response.FillerOrders
					}
				};
			}
			catch (Exception)
			{
				return new ApiSubmitUserTradeResponse { Success = false, Error = "Bad Request" };
			}
		}

		[HttpPost]
		public async Task<ApiCancelUserTradeResponse> CancelTrade(ApiCancelTrade request)
		{
			try
			{
				var response = await ApiPrivateService.CancelUserTrade(new ApiCancelUserTradeRequest
				{
					CancelType = request.Type,
					TradeId = request.OrderId,
					TradePairId = request.TradePairId,
					UserId = new Guid(User.Identity.Name)
				});

				if (response.IsError)
					return new ApiCancelUserTradeResponse { Success = false, Error = response.Error };

				await Task.WhenAll(response.Notifications.SendNotifications(), response.DataUpdates.SendTradeNotifications());

				return new ApiCancelUserTradeResponse
				{
					Success = true,
					Data = response.CanceledOrders,
				};
			}
			catch (Exception)
			{
				return new ApiCancelUserTradeResponse { Success = false, Error = "Bad Request" };
			}
		}

		[HttpPost]
		public async Task<ApiSubmitUserTransferResponse> SubmitTransfer(ApiSubmitTransfer request)
		{
			try
			{
				var userId = new Guid(User.Identity.Name);
				var response = await ApiPrivateService.SubmitUserTransfer(new ApiSubmitUserTransferRequest
				{
					Amount = request.Amount,
					Currency = request.Currency,
					CurrencyId = request.CurrencyId,
					UserName = request.UserName,
					Type = TransferType.User,
					UserId = userId,
				});

				if (response.IsError)
					return new ApiSubmitUserTransferResponse { Success = false, Error = response.Error };

				await response.Notifications.SendNotifications();
				var message = response.Notifications.FirstOrDefault(x => x.UserId == userId);
				return new ApiSubmitUserTransferResponse
				{
					Success = true,
					Data = new ApiUserTransferResult
					{
						TransferId = response.TransferId,
						Message = message?.Notification,
					}
				};
			}
			catch (Exception)
			{
				return new ApiSubmitUserTransferResponse { Success = false, Error = "Bad Request" };
			}
		}

		[HttpPost]
		[EnableThrottling(PerMinute = 1, PerHour = 30)]
		public async Task<ApiSubmitUserTipResponse> SubmitTip(ApiSubmitTip request)
		{
			try
			{
				var response = await ApiPrivateService.SubmitUserTip(new ApiSubmitUserTipRequest
				{
					ActiveUsers = request.ActiveUsers,
					Amount = request.Amount,
					Currency = request.Currency,
					CurrencyId = request.CurrencyId,
					UserId = new Guid(User.Identity.Name)
				});

				if (response.IsError)
					return new ApiSubmitUserTipResponse { Success = false, Error = response.Error };

				await response.Notifications.SendNotifications();
				var message = response.Notifications.FirstOrDefault() as TipNotificationModel;
				return new ApiSubmitUserTipResponse
				{
					Success = true,
					Data = message?.Notification,
				};
			}
			catch (Exception)
			{
				return new ApiSubmitUserTipResponse { Success = false, Error = "Bad Request" };
			}
		}

		[HttpPost]
		public async Task<ApiUserOpenOrdersResponse> GetOpenOrders(ApiOpenOrders request)
		{
			try
			{
				return await ApiPrivateService.GetUserOpenOrders(new ApiUserOpenOrdersRequest
				{
					Count = request.Count,
					Market = request.Market,
					TradePairId = request.TradePairId,
					UserId = new Guid(User.Identity.Name)
				});
			}
			catch (Exception)
			{
				return new ApiUserOpenOrdersResponse { Success = false, Error = "Bad Request" };
			}
		}

		[HttpPost]
		public async Task<ApiUserTradeHistoryResponse> GetTradeHistory(ApiHistory request)
		{
			try
			{
				return await ApiPrivateService.GetUserTradeHistory(new ApiUserTradeHistoryRequest
				{
					Count = request.Count,
					Market = request.Market,
					TradePairId = request.TradePairId,
					UserId = new Guid(User.Identity.Name)
				});
			}
			catch (Exception)
			{
				return new ApiUserTradeHistoryResponse { Success = false, Error = "Bad Request" };
			}
		}

		[HttpPost]
		public async Task<ApiUserBalanceResponse> GetBalance(BalanceRequest request)
		{
			try
			{
				return await ApiPrivateService.GetUserBalance(new ApiUserBalanceRequest
				{
					Currency = request.Currency,
					CurrencyId = request.CurrencyId,
					UserId = new Guid(User.Identity.Name)
				});
			}
			catch (Exception)
			{
				return new ApiUserBalanceResponse { Success = false, Error = "Bad Request" };
			}
		}

		[HttpPost]
		public async Task<ApiUserTransactionsResponse> GetTransactions(ApiTransaction request)
		{
			try
			{
				return await ApiPrivateService.GetUserTransactions(new ApiUserTransactionsRequest
				{
					Count = request.Count,
					Type = request.Type,
					UserId = new Guid(User.Identity.Name)
				});
			}
			catch (Exception)
			{
				return new ApiUserTransactionsResponse { Success = false, Error = "Bad Request" };
			}
		}

		[HttpPost]
		public async Task<ApiUserDepositAddressResponse> GetDepositAddress(ApiDepositAddress request)
		{
			try
			{
				return await ApiPrivateService.GetUserDepositAddress(new ApiUserDepositAddressRequest
				{
					Currency = request.Currency,
					CurrencyId = request.CurrencyId,
					UserId = new Guid(User.Identity.Name)
				});
			}
			catch (Exception)
			{
				return new ApiUserDepositAddressResponse { Success = false, Error = "Bad Request" };
			}
		}

		[HttpPost]
		public async Task<ApiSubmitUserWithdrawResponse> SubmitWithdraw(ApiSubmitWithdraw request)
		{
			try
			{
				return await ApiPrivateService.SubmitUserWithdraw(new ApiSubmitUserWithdrawRequest
				{
					Address = request.Address,
					PaymentId = request.PaymentId,
					Amount = request.Amount,
					Currency = request.Currency,
					CurrencyId = request.CurrencyId,
					UserId = new Guid(User.Identity.Name)
				});
			}
			catch (Exception)
			{
				return new ApiSubmitUserWithdrawResponse { Success = false, Error = "Bad Request" };
			}
		}
	}
}