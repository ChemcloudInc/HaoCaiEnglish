using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class NavigationController : BaseAdminController
	{
		public NavigationController()
		{
		}

		[Description("新增导航")]
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
			ServiceHelper.Create<INavigationService>().AddPlatformNavigation(info);
			Result result1 = new Result()
			{
				success = true,
				msg = "添加导航成功！"
			};
			return Json(result1);
		}

		[Description("删除导航")]
		[HttpPost]
		[OperationLog(Message="删除导航")]
		[UnAuthorize]
		public JsonResult Delete(long id)
		{
			ServiceHelper.Create<INavigationService>().DeletePlatformNavigation(id);
			Result result = new Result()
			{
				success = true,
				msg = "删除成功！"
			};
			return Json(result);
		}

		[Description("编辑导航")]
		[OperationLog(Message="编辑导航")]
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
			ServiceHelper.Create<INavigationService>().UpdatePlatformNavigation(info);
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
			List<BannerInfo> list = ServiceHelper.Create<INavigationService>().GetPlatNavigations().ToList();
			return View(list);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult SwapDisplaySequence(long id, long id2)
		{
			ServiceHelper.Create<INavigationService>().SwapPlatformDisplaySequence(id, id2);
			Result result = new Result()
			{
				success = true,
				msg = "排序成功！"
			};
			return Json(result);
		}
	}
}