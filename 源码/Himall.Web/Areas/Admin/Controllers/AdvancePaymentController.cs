using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using System;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class AdvancePaymentController : BaseAdminController
	{
		public AdvancePaymentController()
		{
		}

		public ActionResult edit()
		{
			SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
			SiteSettingModel siteSettingModel = new SiteSettingModel()
			{
				AdvancePaymentPercent = siteSettings.AdvancePaymentPercent,
				AdvancePaymentLimit = siteSettings.AdvancePaymentLimit,
				UnpaidTimeout = siteSettings.UnpaidTimeout,
				NoReceivingTimeout = siteSettings.NoReceivingTimeout,
				SalesReturnTimeout = siteSettings.SalesReturnTimeout
			};
			return View(siteSettingModel);
		}

		[HttpPost]
		[UnAuthorize]
		[ValidateInput(false)]
		public JsonResult Edit(SiteSettingModel siteSettingModel)
		{
			Result result = new Result();
			SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
			siteSettings.NoReceivingTimeout = siteSettingModel.NoReceivingTimeout;
			siteSettings.UnpaidTimeout = siteSettingModel.UnpaidTimeout;
			siteSettings.SalesReturnTimeout = siteSettingModel.SalesReturnTimeout;
			ServiceHelper.Create<ISiteSettingService>().SetSiteSettings(siteSettings);
			result.success = true;
			return Json(result);
		}
	}
}