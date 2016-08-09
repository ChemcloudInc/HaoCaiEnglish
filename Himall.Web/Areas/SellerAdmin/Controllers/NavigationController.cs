using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class NavigationController : BaseSellerController
	{
		public NavigationController()
		{
		}

		[Description("新增导航")]
		[ShopOperationLog(Message="新增导航")]
		[UnAuthorize]
		public JsonResult Add(BannerInfo info)
		{
			if (string.IsNullOrWhiteSpace(info.Name) || string.IsNullOrWhiteSpace(info.Url))
			{
				Result result = new Result()
				{
					success = false,
					msg = "导航名称和跳转地址不能为空！"
				};
				return Json(result);
			}
			info.ShopId = base.CurrentSellerManager.ShopId;
			ServiceHelper.Create<INavigationService>().AddSellerNavigation(info);
			Result result1 = new Result()
			{
				success = true,
				msg = "添加导航成功！"
			};
			return Json(result1);
		}

		[Description("删除导航")]
		[HttpPost]
		[ShopOperationLog(Message="删除导航")]
		[UnAuthorize]
		public JsonResult Delete(long id)
		{
			ServiceHelper.Create<INavigationService>().DeleteSellerformNavigation(base.CurrentSellerManager.ShopId, id);
			Result result = new Result()
			{
				success = true,
				msg = "删除成功！"
			};
			return Json(result);
		}

		[ShopOperationLog(Message="编辑导航")]
		[UnAuthorize]
		public JsonResult Edit(BannerInfo info)
		{
			if (string.IsNullOrWhiteSpace(info.Name) || string.IsNullOrWhiteSpace(info.Url))
			{
				Result result = new Result()
				{
					success = false,
					msg = "导航名称和跳转地址不能为空！"
				};
				return Json(result);
			}
			info.ShopId = base.CurrentSellerManager.ShopId;
			ServiceHelper.Create<INavigationService>().UpdateSellerNavigation(info);
			Result result1 = new Result()
			{
				success = true,
				msg = "编辑导航成功！"
			};
			return Json(result1);
		}

		[Description("获取导航列表数据")]
		public ActionResult Management()
		{
			List<BannerInfo> list = ServiceHelper.Create<INavigationService>().GetSellerNavigations(base.CurrentSellerManager.ShopId, PlatformType.PC).ToList();
			return View(list);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult SwapDisplaySequence(long id, long id2)
		{
			ServiceHelper.Create<INavigationService>().SwapSellerDisplaySequence(base.CurrentSellerManager.ShopId, id, id2);
			Result result = new Result()
			{
				success = true,
				msg = "排序成功！"
			};
			return Json(result);
		}
	}
}