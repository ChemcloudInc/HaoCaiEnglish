using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.SellerAdmin.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class WeiXinController : BaseSellerController
	{
		public WeiXinController()
		{
		}

		[HttpPost]
		public JsonResult AddMenu(string title, string url, string parentId, int urlType, long? menuId)
		{
			int num;
			VShopInfo vShopByShopId = ServiceHelper.Create<IVShopService>().GetVShopByShopId(base.CurrentSellerManager.ShopId);
			num = (parentId != "0" ? 2 : 1);
			switch (urlType)
			{
				case 1:
				{
					object[] host = new object[] { "http://", base.HttpContext.Request.Url.Host, "/m-", PlatformType.WeiXin.ToString(), "/vshop/detail/", vShopByShopId.Id, "?shop=", base.CurrentSellerManager.ShopId };
					url = string.Concat(host);
					break;
				}
				case 2:
				{
					object[] objArray = new object[] { "http://", base.HttpContext.Request.Url.Host, "/m-", PlatformType.WeiXin.ToString(), "/vshop/category?vshopid=", vShopByShopId.Id, "&shop=", base.CurrentSellerManager.ShopId };
					url = string.Concat(objArray);
					break;
				}
				case 3:
				{
					object[] host1 = new object[] { "http://", base.HttpContext.Request.Url.Host, "/m-", PlatformType.WeiXin.ToString(), "/cart/cart?shop=", base.CurrentSellerManager.ShopId };
					url = string.Concat(host1);
					break;
				}
				case 4:
				{
					object[] objArray1 = new object[] { "http://", base.HttpContext.Request.Url.Host, "/m-", PlatformType.WeiXin.ToString(), "/member/center?shop=", base.CurrentSellerManager.ShopId };
					url = string.Concat(objArray1);
					break;
				}
			}
			if (!string.IsNullOrEmpty(url) && !url.Contains("http://"))
			{
				throw new HimallException("链接必须以http://开头");
			}
			Result result = new Result();
			MenuInfo menuInfo = new MenuInfo()
			{
				Title = title,
				Url = url,
				ParentId = Convert.ToInt64(parentId),
				Platform = PlatformType.WeiXin,
				Depth =(short) num,
				ShopId = base.CurrentSellerManager.ShopId,
				FullIdPath = "1",
				Sequence = 1,
				UrlType = new MenuInfo.UrlTypes?((MenuInfo.UrlTypes)urlType)
			};
			MenuInfo value = menuInfo;
			if (!menuId.HasValue)
			{
				ServiceHelper.Create<IWeixinMenuService>().AddMenu(value);
			}
			else
			{
				value.Id = menuId.Value;
				ServiceHelper.Create<IWeixinMenuService>().UpdateMenu(value);
			}
			result.success = true;
			return Json(result);
		}

		public ActionResult BasicVShopSettings()
		{
			WXShopInfo vShopSetting = ServiceHelper.Create<IVShopService>().GetVShopSetting(base.CurrentSellerManager.ShopId) ?? new WXShopInfo();
			if (string.IsNullOrEmpty(vShopSetting.Token))
			{
				vShopSetting.Token = CreateKey(8);
			}
			ViewBag.Url = string.Format("http://{0}/m-Weixin/WXApi/{1}", base.Request.Url.Host, base.CurrentSellerManager.ShopId);
			ViewBag.VShop = ServiceHelper.Create<IVShopService>().GetVShopByShopId(base.CurrentSellerManager.ShopId);
			ViewBag.ShopId = base.CurrentSellerManager.ShopId;
			return View(vShopSetting);
		}

		private string CreateKey(int len)
		{
			byte[] numArray = new byte[len];
			(new RNGCryptoServiceProvider()).GetBytes(numArray);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < numArray.Length; i++)
			{
				stringBuilder.Append(string.Format("{0:X2}", numArray[i]));
			}
			return stringBuilder.ToString();
		}

		[HttpPost]
		public JsonResult DeleteMenu(int menuId)
		{
			Result result = new Result();
			ServiceHelper.Create<IWeixinMenuService>().DeleteMenu(menuId);
			result.success = true;
			return Json(result);
		}

		[HttpPost]
		public JsonResult GetMenu(long? menuId)
		{
			MenuInfo menuInfo = new MenuInfo();
			if (menuId.HasValue)
			{
				menuInfo = ServiceHelper.Create<IWeixinMenuService>().GetMenu(menuId.Value);
			}
			return Json(new { success = true, title = menuInfo.Title, urlType = menuInfo.UrlType, url = menuInfo.Url, parentId = menuInfo.ParentId });
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult MenuManage()
		{
			List<MenuManageModel> menuManageModels = new List<MenuManageModel>();
			foreach (MenuInfo mainMenu in ServiceHelper.Create<IWeixinMenuService>().GetMainMenu(base.CurrentSellerManager.ShopId))
			{
				MenuManageModel menuManageModel = new MenuManageModel()
				{
					ID = mainMenu.Id,
					TopMenuName = mainMenu.Title,
					SubMenu = ServiceHelper.Create<IWeixinMenuService>().GetMenuByParentId(mainMenu.Id),
					URL = mainMenu.Url,
					LinkType = mainMenu.UrlType
				};
				menuManageModels.Add(menuManageModel);
			}
			ViewBag.ShopId = base.CurrentSellerManager.ShopId;
			ViewBag.VShop = ServiceHelper.Create<IVShopService>().GetVShopByShopId(base.CurrentSellerManager.ShopId);
			return View(menuManageModels);
		}

		[HttpPost]
		public JsonResult RequestToWeixin()
		{
			Result result = new Result();
			ServiceHelper.Create<IWeixinMenuService>().ConsistentToWeixin(base.CurrentSellerManager.ShopId);
			result.success = true;
			return Json(result);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult SaveVShopSettings(string weixinAppId, string weixinAppSecret, string weixinfollowUrl, string weixiToken)
		{
			Result result = new Result();
			WXShopInfo wXShopInfo = new WXShopInfo()
			{
				ShopId = base.CurrentSellerManager.ShopId,
				AppId = weixinAppId,
				AppSecret = weixinAppSecret,
				FollowUrl = weixinfollowUrl,
				Token = weixiToken
			};
			ServiceHelper.Create<IVShopService>().SaveVShopSetting(wXShopInfo);
			result.success = true;
			return Json(result);
		}
	}
}