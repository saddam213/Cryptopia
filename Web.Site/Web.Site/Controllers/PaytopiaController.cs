using Cryptopia.Common.Paytopia;
using Cryptopia.Common.User;
using Cryptopia.Enums;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Site.Extensions;
using Web.Site.Helpers;

namespace Web.Site.Controllers
{
	public class PaytopiaController : BaseController
	{
		public IPaytopiaReader PaytopiaReader { get; set; }
		public IPaytopiaWriter PaytopiaWriter { get; set; }
		public IUserBalanceReader UserBalanceReader { get; set; }

		public async Task<ActionResult> Index()
		{
			var items = await PaytopiaReader.GetItems();
			return View(new PaytopiaModel
			{
				ComboListing = items.FirstOrDefault(x => x.Type == PaytopiaItemType.ComboListing),
				ExchangeListing = items.FirstOrDefault(x => x.Type == PaytopiaItemType.ExchangeListing),
				FeaturedCurrency = items.FirstOrDefault(x => x.Type == PaytopiaItemType.FeaturedCurrency),
				FeaturedPool = items.FirstOrDefault(x => x.Type == PaytopiaItemType.FeaturedPool),
				LottoSlot = items.FirstOrDefault(x => x.Type == PaytopiaItemType.LottoSlot),
				PoolListing = items.FirstOrDefault(x => x.Type == PaytopiaItemType.PoolListing),
				RewardSlot = items.FirstOrDefault(x => x.Type == PaytopiaItemType.RewardSlot),
				TipSlot = items.FirstOrDefault(x => x.Type == PaytopiaItemType.TipSlot),
				Shares = items.FirstOrDefault(x => x.Type == PaytopiaItemType.Shares),
				TwoFactor = items.FirstOrDefault(x => x.Type == PaytopiaItemType.TwoFactor)
			});
		}


		#region Featured Slot

		[HttpGet]
		[AuthorizeAjax]
		public async Task<ActionResult> FeaturedPoolSlot()
		{
			var item = await PaytopiaReader.GetItem(PaytopiaItemType.FeaturedPool);
			var items = await PaytopiaReader.GetFeaturedPoolSlotItems();
			var balance = await UserBalanceReader.GetBalance(User.Identity.GetUserId(), item.CurrencyId);
			return View("FeaturedPoolSlotModal", new FeaturedSlotModel
			{
				Items = items,
				Balance = balance?.Available ?? 0,
				Currency = item.Symbol,
				Price = item.Price,
				Name = item.Name,
				Description = item.Description
			});
		}

		[HttpPost]
		[AuthorizeAjax]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> FeaturedPoolSlot(FeaturedSlotModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Items = await PaytopiaReader.GetFeaturedPoolSlotItems();
				return View("FeaturedPoolSlotModal", model);
			}

			var result = await PaytopiaWriter.UpdateFeaturedPoolSlot(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
			{
				model.Items = await PaytopiaReader.GetFeaturedPoolSlotItems();
				return View("FeaturedPoolSlotModal", model);
			}

			return CloseModal(result);
		}

		[HttpGet]
		[AuthorizeAjax]
		public async Task<ActionResult> FeaturedCurrencySlot()
		{
			var item = await PaytopiaReader.GetItem(PaytopiaItemType.FeaturedCurrency);
			var items = await PaytopiaReader.GetFeaturedCurrencySlotItems();
			var balance = await UserBalanceReader.GetBalance(User.Identity.GetUserId(), item.CurrencyId);
			return View("FeaturedCurrencySlotModal", new FeaturedSlotModel
			{
				Items = items,
				Balance = balance?.Available ?? 0,
				Currency = item.Symbol,
				Price = item.Price,
				Name = item.Name,
				Description = item.Description
			});
		}

		[HttpPost]
		[AuthorizeAjax]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> FeaturedCurrencySlot(FeaturedSlotModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Items = await PaytopiaReader.GetFeaturedCurrencySlotItems();
				return View("FeaturedCurrencySlotModal", model);
			}

			var result = await PaytopiaWriter.UpdateFeaturedCurrencySlot(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
			{
				model.Items = await PaytopiaReader.GetFeaturedCurrencySlotItems();
				return View("FeaturedCurrencySlotModal", model);
			}

			return CloseModal(result);
		}

		#endregion

		#region Combo Listing

		[HttpGet]
		[AuthorizeAjax]
		public async Task<ActionResult> ComboListing()
		{
			var item = await PaytopiaReader.GetItem(PaytopiaItemType.ComboListing);
			var balance = await UserBalanceReader.GetBalance(User.Identity.GetUserId(), item.CurrencyId);
			return View("ExchangeComboListingModal", new ExchangeListingModel
			{
				Balance = balance?.Available ?? 0,
				Currency = item.Symbol,
				Price = item.Price,
				Name = item.Name,
				Description = item.Description
			});
		}

		[HttpPost]
		[AuthorizeAjax]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ComboListing(ExchangeListingModel model)
		{
			if (!ModelState.IsValid)
			{
				return View("ExchangeComboListingModal", model);
			}

			var result = await PaytopiaWriter.UpdateComboListing(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
			{
				return View("ExchangeComboListingModal", model);
			}

			return CloseModal(result);
		}

		#endregion

		#region Exchange Listing

		[HttpGet]
		[AuthorizeAjax]
		public async Task<ActionResult> ExchangeListing()
		{
			var item = await PaytopiaReader.GetItem(PaytopiaItemType.ExchangeListing);
			var balance = await UserBalanceReader.GetBalance(User.Identity.GetUserId(), item.CurrencyId);
			return View("ExchangeListingModal", new ExchangeListingModel
			{
				Balance = balance?.Available ?? 0,
				Currency = item.Symbol,
				Price = item.Price,
				Name = item.Name,
				Description = item.Description
			});
		}

		[HttpPost]
		[AuthorizeAjax]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ExchangeListing(ExchangeListingModel model)
		{
			if (!ModelState.IsValid)
				return View("ExchangeListingModal", model);

			var result = await PaytopiaWriter.UpdateExchangeListing(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("ExchangeListingModal", model);

			return CloseModal(result);
		}

		#endregion

		#region Pool Listing

		[HttpGet]
		[AuthorizeAjax]
		public async Task<ActionResult> PoolListing()
		{
			var item = await PaytopiaReader.GetItem(PaytopiaItemType.PoolListing);
			var items = await PaytopiaReader.GetPoolListingItems();
			var balance = await UserBalanceReader.GetBalance(User.Identity.GetUserId(), item.CurrencyId);
			return View("PoolListingModal", new PoolListingModel
			{
				Balance = balance?.Available ?? 0,
				Currency = item.Symbol,
				Price = item.Price,
				Name = item.Name,
				Items = items,
				Description = item.Description
			});
		}

		[HttpPost]
		[AuthorizeAjax]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> PoolListing(PoolListingModel model)
		{
			if (!model.IsListed && !ModelState.IsValid)
			{
				model.Items = await PaytopiaReader.GetPoolListingItems();
				return View("PoolListingModal", model);
			}

			ModelState.Clear();
			var result = await PaytopiaWriter.UpdatePoolListing(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
			{
				model.Items = await PaytopiaReader.GetPoolListingItems();
				return View("PoolListingModal", model);
			}

			return CloseModal(result);
		}

		#endregion

		#region Lotto Slot

		[HttpGet]
		[AuthorizeAjax]
		public async Task<ActionResult> LottoSlot()
		{
			var item = await PaytopiaReader.GetItem(PaytopiaItemType.LottoSlot);
			var items = await PaytopiaReader.GetLottoSlotItems();
			var balance = await UserBalanceReader.GetBalance(User.Identity.GetUserId(), item.CurrencyId);
			return View("LottoSlotModal", new LottoSlotModel
			{
				Items = items,
				Balance = balance?.Available ?? 0,
				Currency = item.Symbol,
				Price = item.Price,
				Name = item.Name,
				Description = item.Description
			});
		}

		[HttpPost]
		[AuthorizeAjax]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> LottoSlot(LottoSlotModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Items = await PaytopiaReader.GetLottoSlotItems();
				return View("LottoSlotModal", model);
			}


			var result = await PaytopiaWriter.UpdateLottoSlot(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
			{
				model.Items = await PaytopiaReader.GetLottoSlotItems();
				return View("LottoSlotModal", model);
			}

			return CloseModal(result);
		}

		#endregion

		#region Reward Slot

		[HttpGet]
		[AuthorizeAjax]
		public async Task<ActionResult> RewardSlot()
		{
			var item = await PaytopiaReader.GetItem(PaytopiaItemType.RewardSlot);
			var items = await PaytopiaReader.GetRewardSlotItems();
			var balance = await UserBalanceReader.GetBalance(User.Identity.GetUserId(), item.CurrencyId);
			return View("RewardSlotModal", new RewardSlotModel
			{
				Items = items,
				Balance = balance?.Available ?? 0,
				Currency = item.Symbol,
				Price = item.Price,
				Name = item.Name,
				Description = item.Description
			});
		}

		[HttpPost]
		[AuthorizeAjax]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> RewardSlot(RewardSlotModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Items = await PaytopiaReader.GetRewardSlotItems();
				return View("RewardSlotModal", model);
			}

			var result = await PaytopiaWriter.UpdateRewardSlot(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
			{
				model.Items = await PaytopiaReader.GetRewardSlotItems();
				return View("RewardSlotModal", model);
			}

			return CloseModal(result);
		}

		#endregion

		#region Tip Slot

		[HttpGet]
		[AuthorizeAjax]
		public async Task<ActionResult> TipSlot()
		{
			var item = await PaytopiaReader.GetItem(PaytopiaItemType.TipSlot);
			var items = await PaytopiaReader.GetTipSlotItems();
			var balance = await UserBalanceReader.GetBalance(User.Identity.GetUserId(), item.CurrencyId);
			return View("TipSlotModal", new TipSlotModel
			{
				Items = items,
				Balance = balance?.Available ?? 0,
				Currency = item.Symbol,
				Price = item.Price,
				Name = item.Name,
				Description = item.Description
			});
		}

		[HttpPost]
		[AuthorizeAjax]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> TipSlot(TipSlotModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Items = await PaytopiaReader.GetTipSlotItems();
				return View("TipSlotModal", model);
			}

			var result = await PaytopiaWriter.UpdateTipSlot(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
			{
				model.Items = await PaytopiaReader.GetTipSlotItems();
				return View("TipSlotModal", model);
			}

			return CloseModal(result);
		}

		#endregion

		#region Shares

		[HttpGet]
		[AuthorizeAjax]
		public async Task<ActionResult> Shares()
		{
			var item = await PaytopiaReader.GetItem(PaytopiaItemType.Shares);
			var balance = await UserBalanceReader.GetBalance(User.Identity.GetUserId(), item.CurrencyId);
			return View("SharesModal", new SharesModel
			{
				Balance = balance?.Available ?? 0,
				Currency = item.Symbol,
				Price = item.Price,
				Name = item.Name,
				Description = item.Description
			});
		}

		[HttpPost]
		[AuthorizeAjax]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Shares(SharesModel model)
		{
			if (!ClaimsUser.IsShareholder && !ModelState.IsValid)
			{
				return View("SharesModal", model);
			}

			ModelState.Clear();
			var result = await PaytopiaWriter.UpdateShares(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
			{
				return View("SharesModal", model);
			}

			return CloseModal(result);
		}

		#endregion

		#region TwoFactor

		[HttpGet]
		[AuthorizeAjax]
		public async Task<ActionResult> TwoFactor()
		{
			var item = await PaytopiaReader.GetItem(PaytopiaItemType.TwoFactor);
			var balance = await UserBalanceReader.GetBalance(User.Identity.GetUserId(), item.CurrencyId);
			return View("TwoFactorModal", new TwoFactorModel
			{
				Balance = balance?.Available ?? 0,
				Currency = item.Symbol,
				Price = item.Price,
				Name = item.Name,
				Description = item.Description
			});
		}

		[HttpPost]
		[AuthorizeAjax]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> TwoFactor(TwoFactorModel model)
		{
			if (!ModelState.IsValid)
			{
				return View("TwoFactorModal", model);
			}

			ModelState.Clear();
			var result = await PaytopiaWriter.UpdateTwoFactor(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
			{
				return View("TwoFactorModal", model);
			}

			return CloseModal(result);
		}

		#endregion
	}
}