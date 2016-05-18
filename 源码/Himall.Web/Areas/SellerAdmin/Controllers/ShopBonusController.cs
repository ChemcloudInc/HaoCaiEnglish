using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class ShopBonusController : BaseSellerController
	{
		private IShopBonusService _bonusService;

		public ShopBonusController()
		{
            _bonusService = ServiceHelper.Create<IShopBonusService>();
		}

		public ActionResult Add()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Add(ShopBonusModel model)
		{
			string mapPath = IOHelper.GetMapPath(model.ShareImg);
			Guid guid = Guid.NewGuid();
			string str = string.Concat(guid.ToString(), (new FileInfo(mapPath)).Extension);
			string mapPath1 = IOHelper.GetMapPath("/Storage/Shop/Bonus");
			if (!Directory.Exists(mapPath1))
			{
				Directory.CreateDirectory(mapPath1);
			}
			IOHelper.CopyFile(mapPath, mapPath1, false, str);
			model.ShareImg = string.Concat("/Storage/Shop/Bonus/", str);
            _bonusService.Add(model, base.CurrentSellerManager.ShopId);
			return RedirectToAction("Management");
		}

        public ActionResult BuyService()
        {
            ActiveMarketServiceInfo shopBonusService = _bonusService.GetShopBonusService(base.CurrentSellerManager.ShopId);
            MarketSettingInfo serviceSetting = ServiceHelper.Create<IMarketService>().GetServiceSetting((MarketType)4);
            ((dynamic)base.ViewBag).Market = shopBonusService;
            ((dynamic)base.ViewBag).IsNo = true;
            string str = null;
            DateTime date = DateTime.Now.Date;
            if ((shopBonusService != null) && (shopBonusService.MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>(item => item.EndTime) < date))
            {
                str = "您的随机红包服务已经过期，您可以续费。";
            }
            else if ((shopBonusService != null) && (shopBonusService.MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>(item => item.EndTime) >= date))
            {
                DateTime time2 = shopBonusService.MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>(item => item.EndTime);
                str = string.Format("{0} 年 {1} 月 {2} 日", time2.Year, time2.Month, time2.Day);
            }
            else if (serviceSetting == null)
            {
                ((dynamic)base.ViewBag).IsNo = false;
                return View();
            }
            ViewBag.EndDate = str;
            ViewBag.Price = serviceSetting.Price;
            return View();
        }


        [HttpPost]
		[UnAuthorize]
		public JsonResult BuyService(int month)
		{
			Result result = new Result();
			IMarketService marketService = ServiceHelper.Create<IMarketService>();
			marketService.OrderMarketService(month, base.CurrentSellerManager.ShopId, MarketType.RandomlyBonus);
			result.success = true;
			result.msg = "购买服务成功";
			return Json(result);
		}

		public ActionResult Detail(long id)
		{
			ViewBag.Id = id;
			return View();
		}

		[HttpPost]
		public ActionResult DetailList(long id, int page = 1, int rows = 20)
		{
			PageModel<ShopBonusReceiveInfo> detail = _bonusService.GetDetail(id, page, rows);
			List<ShopBonusReceiveModel> list = (
				from p in detail.Models.ToList()
				select new ShopBonusReceiveModel()
				{
					OpenId = p.OpenId,
					Price = p.Price.Value,
					ReceiveTime = (!p.ReceiveTime.HasValue ? "" : p.ReceiveTime.Value.ToString("yyyy-MM-dd")),
					StateStr = p.State.ToDescription(),
					UsedTime = (!p.UsedTime.HasValue ? "" : p.UsedTime.Value.ToString("yyyy-MM-dd")),
					UsedOrderId = p.UsedOrderId.ToString()
				}).ToList();
			DataGridModel<ShopBonusReceiveModel> dataGridModel = new DataGridModel<ShopBonusReceiveModel>()
			{
				rows = list,
				total = detail.Total
			};
			return Json(dataGridModel);
		}

		public ActionResult Edit(long id)
		{
			ShopBonusInfo shopBonusInfo = _bonusService.Get(id);
			return View(new ShopBonusModel(shopBonusInfo));
		}

		[HttpPost]
		public ActionResult EditData(ShopBonusModel model)
		{
			ActiveMarketServiceInfo shopBonusService = _bonusService.GetShopBonusService(base.CurrentSellerManager.ShopId);
			DateTime dateTime = shopBonusService.MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>((MarketServiceRecordInfo a) => a.EndTime);
			if (model.BonusDateEnd > dateTime || model.DateEnd > dateTime)
			{
				throw new HimallException("随机红包截止日期不允许超过您购买的服务到期时间。");
			}
            _bonusService.Update(model);
			return RedirectToAction("Management");
		}

		[HttpPost]
		public JsonResult Invalid(long id)
		{
            _bonusService.Invalid(id);
			return Json(true);
		}

		[HttpPost]
		public ActionResult IsAdd()
		{
			return Json(_bonusService.IsAdd(base.CurrentSellerManager.ShopId));
		}

		[HttpPost]
		public ActionResult IsOverDate(string bend, string end)
		{
			bool flag = _bonusService.IsOverDate(DateTime.Parse(bend), DateTime.Parse(end), base.CurrentSellerManager.ShopId);
			return Json(flag);
		}

		[HttpPost]
		public ActionResult List(string name, int state, int page = 1, int rows = 20)
		{
			PageModel<ShopBonusInfo> pageModel = _bonusService.Get(base.CurrentSellerManager.ShopId, name.Trim(), state, page, rows);
			List<ShopBonusModel> shopBonusModels = new List<ShopBonusModel>();
			foreach (ShopBonusInfo model in pageModel.Models)
			{
				shopBonusModels.Add(new ShopBonusModel(model));
			}
			DataGridModel<ShopBonusModel> dataGridModel = new DataGridModel<ShopBonusModel>()
			{
				rows = shopBonusModels,
				total = pageModel.Total
			};
			return Json(dataGridModel);
		}

		public ActionResult Management()
		{
			if (ServiceHelper.Create<IMarketService>().GetServiceSetting(MarketType.Coupon) == null)
			{
				throw new HimallException("平台未配置随机红包");
			}
			ActiveMarketServiceInfo shopBonusService = _bonusService.GetShopBonusService(base.CurrentSellerManager.ShopId);
			if (shopBonusService != null)
			{
				if (shopBonusService != null)
				{
					if (shopBonusService.MarketServiceRecordInfo.Max<MarketServiceRecordInfo, DateTime>((MarketServiceRecordInfo a) => a.EndTime) < DateTime.Now.Date)
					{
						return RedirectToAction("BuyService");
					}
				}
				ViewBag.Market = shopBonusService;
				return View();
			}
			return RedirectToAction("BuyService");
		}

		public ActionResult UnConfig()
		{
			return View();
		}
	}
}