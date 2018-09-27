using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using Web.Site.Models;
using Microsoft.AspNet.Identity;
using Cryptopia.Data.DataContext;
using System.Data.Entity;
using Web.Site.Notifications;
using Cryptopia.Entity;
using Cryptopia.Enums;
using Cryptopia.Common.Chat;
using Cryptopia.Common.Address;
using Cryptopia.Common.Balance;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Currency;
using Cryptopia.Common.Trade;
using Cryptopia.Common.Withdraw;
using Cryptopia.Base;
using Cryptopia.Common.Transfer;

namespace Web.Site.Controllers
{
	/// <summary>
	/// Controller for Balance specific actions
	/// </summary>
	public class BalanceController : BaseUserController
	{
		public ICurrencyReader CurrencyReader { get; set; }
		public IBalanceReader BalanceReader { get; set; }
		public ITradeService TradeService { get; set; }

		#region Balance

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetCurrencyBalance(int id)
		{
			var balanceModel = await BalanceReader.GetCurrencyBalance(User.Identity.GetUserId(), id);
			return Json(new
			{
				CurrencyId = balanceModel.CurrencyId,
				Symbol = balanceModel.Symbol,
				MinTipAmount = balanceModel.MinTipAmount.ToString("F8"),
				Success = true,
				Balance = balanceModel.Available.ToString("F8"),
			});
		}



		#endregion

		#region Tipping

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> TipCurrency()
		{
			var user = await UserManager.FindValidByIdAsync(User.Identity.GetUserId());
			if (user == null)
			{
				return View("~/Views/Modal/Invalid.cshtml");
			}

			var model = user.GetTwoFactorModel<TipbotModel>(TwoFactorComponent.Tip);
			model.Currencies = await CurrencyReader.GetCurrencies();
			return View(model);
		}


		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitTip(TipbotModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindValidByIdAsync(User.Identity.GetUserId());
				if (user.ChatTipBanEndTime.HasValue && user.ChatTipBanEndTime.Value > DateTime.UtcNow)
				{
					return JsonError(string.Format(Resources.UserWallet.tipBanError,
							         (user.ChatTipBanEndTime.Value - DateTime.UtcNow).TotalSeconds));
				}

				// Verify two factor
				if (!await UserManager.VerifyUserTwoFactorCodeAsync(TwoFactorComponent.Tip, user.Id, model.Code1, model.Code2))
				{
					return JsonError(Resources.UserWallet.tipTwoFactorError);
				}

				var currency = await CurrencyReader.GetCurrency(model.CurrencyId);
				if (currency == null || currency.TippingExpires < DateTime.UtcNow)
				{
					return JsonError(Resources.UserWallet.tipCurrencyIsDisabledError);
				}

				var chatBotId = Constant.SYSTEM_USER_CHATBOT.ToString();
				var ignoreList = new List<string> { user.UserName };
				if (!string.IsNullOrEmpty(user.ChatTipIgnoreList))
				{
					ignoreList.AddRange(
						user.ChatTipIgnoreList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
				}


				if (model.TipPayoutType == TipbotPayoutType.SingleUser)
				{
					// Single user
					if (string.IsNullOrEmpty(model.ChatHandle))
					{
						return JsonError(Resources.UserWallet.tipChatHandleIsRequiredError);
					}

					if (model.Amount < currency.TipMin)
					{
						return JsonError(string.Format(Resources.UserWallet.tipMinAmountError, currency.TipMin, currency.Symbol));
					}

					var userToTip =
						await
							UserManager.Users.FirstOrDefaultAsync(
								x => x.ChatHandle.Equals(model.ChatHandle, StringComparison.OrdinalIgnoreCase));
					if (userToTip == null)
					{
						return JsonError(Resources.UserWallet.tipChatHandleNotFoundError);
					}

					if (userToTip.DisableTips)
					{
						return JsonError(Resources.UserWallet.tipUserDisabledError);
					}

					if (userToTip.ChatTipBanEndTime.HasValue && userToTip.ChatTipBanEndTime.Value > DateTime.UtcNow)
					{
						return JsonError(Resources.UserWallet.tipUserIsBannedError);
					}

					if (ignoreList.Contains(userToTip.UserName))
					{
						return JsonError(Resources.UserWallet.tipUserIsIgnoredError);
					}

					var response = await TradeService.CreateTip(User.Identity.GetUserId(), new CreateTipModel
					{
						UserTo = new List<Guid> { new Guid(userToTip.Id) },
						Amount = model.Amount,
						CurrencyId = currency.CurrencyId,
					});

					if (!string.IsNullOrEmpty(response.Error))
					{
						return JsonError(response.Error);
					}

					await response.Notifications.SendNotifications();
					await ChatHub.SendTipbotMessage(user.ChatHandle, 
						string.Format(Resources.UserWallet.tipTipbotMessage, user.ChatHandle, userToTip.ChatHandle, model.Amount.ToString("F8"), currency.Symbol, 
						              (!string.IsNullOrEmpty(model.Reason) ? string.Format(Resources.UserWallet.tipTipbotReasonMessage, model.Reason) : string.Empty))
					);
					return JsonSuccess(string.Format(Resources.UserWallet.tipResponseMessage, userToTip.ChatHandle, model.Amount.ToString("F8"), currency.Symbol));
				}
				else if (model.TipPayoutType == TipbotPayoutType.MultipleUser)
				{
					// Multiple users
					if (string.IsNullOrEmpty(model.ChatHandles))
					{
						return JsonError(Resources.UserWallet.tipChatHandleIsRequiredError);
					}

					var multiUsers = new List<string>(model.ChatHandles.Split(',').Select(x => x.Trim()).Distinct());
					var usersToTip = await UserManager.Users.Where(x => multiUsers.Contains(x.ChatHandle)).ToListAsync();
					var invalidUsers = new List<string>();
					foreach (var userToTip in usersToTip)
					{
						if (userToTip.DisableTips || ignoreList.Contains(userToTip.UserName) ||
								(userToTip.ChatTipBanEndTime.HasValue && userToTip.ChatTipBanEndTime.Value > DateTime.UtcNow))
						{
							invalidUsers.Add(userToTip.ChatHandle);
						}
					}
					usersToTip = usersToTip.Where(x => !invalidUsers.Contains(x.ChatHandle) && x.Id != chatBotId).ToList();
					if (!usersToTip.Any())
					{
						return JsonError(Resources.UserWallet.tipNoValidUserError);
					}

					if (usersToTip.Count > 100)
					{
						return JsonError(String.Format(Resources.UserWallet.tipMaxUserCountError, 100));
					}

					if (model.Amount < (currency.TipMin * usersToTip.Count))
					{
						return JsonError(string.Format(Resources.UserWallet.tipMinAmountError, currency.TipMin, currency.Symbol));
					}

					var tipAmount = model.Amount / usersToTip.Count;
					var response = await TradeService.CreateTip(User.Identity.GetUserId(), new CreateTipModel
					{
						UserTo = new List<Guid>(usersToTip.Select(x => x.Id).Select(Guid.Parse)),
						Amount = model.Amount,
						CurrencyId = currency.CurrencyId,
					});

					if (!string.IsNullOrEmpty(response.Error))
					{
						return JsonError(response.Error);
					}

					await response.Notifications.SendNotifications();
					await
						ChatHub.SendTipbotMessage(user.ChatHandle,
							string.Format(Resources.UserWallet.tipMultiTipbotMessage, user.ChatHandle,
								string.Join(",", usersToTip.Select(x => x.ChatHandle)), tipAmount.ToString("F8"), currency.Symbol,
								(!string.IsNullOrEmpty(model.Reason) ? string.Format(Resources.UserWallet.tipTipbotReasonMessage, model.Reason) : string.Empty))
						);
					return
						JsonSuccess(string.Format(Resources.UserWallet.tipMultiResponseMessage, string.Join(",", usersToTip.Select(x => x.ChatHandle)),
							tipAmount.ToString("F8"), currency.Symbol));
				}
				else if (model.TipPayoutType == TipbotPayoutType.ActiveChatUsers)
				{
					// Active users
					using (var context = new ApplicationDbContext())
					{
						var usersToTip = new HashSet<string>();
						var users = context.ChatMessages
							.Where(
								x =>
									!x.User.DisableTips && !(x.User.ChatTipBanEndTime.HasValue && x.User.ChatTipBanEndTime.Value > DateTime.UtcNow) &&
									!ignoreList.Contains(x.User.UserName) && x.UserId != chatBotId)
							.OrderByDescending(x => x.Timestamp)
							.ToList();
						if (!users.Any())
						{
							return JsonError(Resources.UserWallet.tipNoValidUserError);
						}

						foreach (var u in users)
						{
							if (usersToTip.Count == model.ActiveMin)
							{
								break;
							}
							usersToTip.Add(u.UserId);
						}

						if (usersToTip.Count > 100)
						{
							return JsonError(String.Format(Resources.UserWallet.tipMaxUserCountError, 100));
						}

						if (model.Amount < (currency.TipMin * usersToTip.Count))
						{
							return JsonError(string.Format(Resources.UserWallet.tipMinAmountError, currency.TipMin, currency.Symbol));
						}

						var tipAmount = model.Amount / usersToTip.Count;
						var response = await TradeService.CreateTip(User.Identity.GetUserId(), new CreateTipModel
						{
							UserTo = new List<Guid>(usersToTip.Select(Guid.Parse)),
							Amount = model.Amount,
							CurrencyId = currency.CurrencyId,
						});
						if (!string.IsNullOrEmpty(response.Error))
						{
							return JsonError(response.Error);
						}

						await response.Notifications.SendNotifications();
						await ChatHub.SendTipbotMessage(user.ChatHandle, 
							string.Format(Resources.UserWallet.tipChatTipbotMessage, user.ChatHandle, usersToTip.Count, tipAmount.ToString("F8"), currency.Symbol, 
							(!string.IsNullOrEmpty(model.Reason) ? string.Format(Resources.UserWallet.tipTipbotReasonMessage, model.Reason) : string.Empty))
						);
						return JsonSuccess(string.Format(Resources.UserWallet.tipChatResponseMessage, usersToTip.Count, tipAmount.ToString("F8"), currency.Symbol));
					}
				}
			}
			return JsonError(ModelState.FirstError());
		}

		#endregion
	
	}
}