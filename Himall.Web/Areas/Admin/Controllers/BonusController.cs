using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.ServiceProvider;
using Himall.Web.Areas.Admin.Models.Market;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class BonusController : BaseAdminController
	{
		private IBonusService _bonusService;

		private SiteSettingsInfo siteSetting;

		public BonusController()
		{
            siteSetting = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
            _bonusService = ServiceHelper.Create<IBonusService>();
		}

		public ActionResult Add()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Add(BonusModel model)
		{
			if (string.IsNullOrEmpty(model.ImagePath))
			{
				model.ImagePath = "";
			}
			else
			{
				string mapPath = IOHelper.GetMapPath(model.ImagePath);
				string name = (new FileInfo(mapPath)).Name;
				IOHelper.CopyFile(mapPath, IOHelper.GetMapPath("/Storage/Plat/Bonus"), false, name);
				model.ImagePath = string.Concat("/Storage/Plat/Bonus/", name);
			}
			string str = string.Concat("http://", base.Request.Url.Host.ToString(), "/m-weixin/bonus/index/");
            _bonusService.Add(model, str);
			return RedirectToAction("Management");
		}

		public ActionResult Apportion(long id)
		{
			BonusInfo bonusInfo = _bonusService.Get(id);
			return View(new BonusModel(bonusInfo));
		}

		[HttpPost]
		public ActionResult CanAdd()
		{
			return Json(_bonusService.CanAddBonus());
		}

		public ActionResult Config()
		{
			return View(siteSetting);
		}

		[HttpPost]
		public ActionResult Config(FormCollection form)
		{
			string item = form["WX_MSGGetCouponTemplateId"];
			ServiceHelper.Create<ISiteSettingService>().SaveSetting("WX_MSGGetCouponTemplateId", item);
			return RedirectToAction("Config");
		}

		public ActionResult Detail(long id)
		{
			ViewBag.Id = id;
			return View();
		}

		[HttpPost]
		public JsonResult DetailList(long id, int page = 1, int rows = 20)
		{
			PageModel<BonusReceiveInfo> detail = _bonusService.GetDetail(id, page, rows);
			List<BonusReceiveModel> list = (
				from p in detail.Models.ToList()
				select new BonusReceiveModel()
				{
					OpenId = p.OpenId,
					Price = p.Price,
					ReceiveTime = (!p.ReceiveTime.HasValue ? "" : p.ReceiveTime.Value.ToString("yyyy-MM-dd")),
					UserName = (p.Himall_Members == null ? "-" : p.Himall_Members.UserName),
					IsTransformedDeposit = p.IsTransformedDeposit
				}).ToList();
			DataGridModel<BonusReceiveModel> dataGridModel = new DataGridModel<BonusReceiveModel>()
			{
				rows = list,
				total = detail.Total
			};
			return Json(dataGridModel);
		}

		public ActionResult Edit(long id)
		{
			BonusInfo bonusInfo = _bonusService.Get(id);
			return View(new BonusModel(bonusInfo));
		}

		[HttpPost]
		public ActionResult EditData(BonusModel model)
		{
			if (!base.ModelState.IsValid)
			{
				throw new HimallException("验证失败");
			}
            _bonusService.Update(model);
			return RedirectToAction("Management");
		}

		private string GetStateString(BonusInfo m)
		{
			if (m.EndTime < DateTime.Now)
			{
				return "已过期";
			}
			if (m.IsInvalid)
			{
				return "已失效";
			}
			return "正在进行";
		}

		[HttpPost]
		public JsonResult Invalid(long id)
		{
            _bonusService.Invalid(id);
			return Json(true);
		}

		[HttpPost]
		public JsonResult List(int type, int state, string name, int page = 1, int rows = 20)
		{
			PageModel<BonusInfo> pageModel = _bonusService.Get(type, state, name.Trim(), page, rows);
			List<BonusModel> list = (
				from m in pageModel.Models.ToList()
				select new BonusModel()
				{
					Id = m.Id,
					Type = m.Type,
					TypeStr = m.TypeStr,
					Style = m.Style,
					PriceType = m.PriceType.Value,
					Name = m.Name,
					MerchantsName = m.MerchantsName,
					Remark = m.Remark,
					Blessing = m.Blessing,
					TotalPrice = m.TotalPrice,
					StartTime = m.StartTime,
					EndTime = m.EndTime,
					EndTimeStr = m.EndTimeStr,
					StartTimeStr = m.StartTimeStr,
					FixedAmount = m.FixedAmount.Value,
					RandomAmountStart = m.RandomAmountStart.Value,
					RandomAmountEnd = m.RandomAmountEnd.Value,
					ImagePath = m.ImagePath,
					Description = m.Description,
					ReceiveCount = m.ReceiveCount,
					IsInvalid = m.IsInvalid,
					StateStr = GetStateString(m)
				}).ToList();
			DataGridModel<BonusModel> dataGridModel = new DataGridModel<BonusModel>()
			{
				rows = list,
				total = pageModel.Total
			};
			return Json(dataGridModel);
		}

		public ActionResult Management()
		{
			ActionResult action;
			try
			{
				SiteSettingsInfo siteSettings = Instance<ISiteSettingService>.Create.GetSiteSettings();
				if (string.IsNullOrEmpty(siteSettings.WeixinAppId) || string.IsNullOrEmpty(siteSettings.WeixinAppSecret))
				{
					action = RedirectToAction("UnConfig");
				}
				else
				{
					action = View();
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Log.Info("微信红包进入出错：", (exception.InnerException == null ? exception : exception.InnerException));
				throw exception;
			}
			return action;
		}

		public ActionResult UnConfig()
		{
			return View();
		}
	}
}