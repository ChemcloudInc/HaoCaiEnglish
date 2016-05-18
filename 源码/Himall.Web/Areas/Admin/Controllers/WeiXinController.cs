using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Admin.Models;
using Himall.Web.Framework;
using Himall.Web.Models;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class WeiXinController : BaseAdminController
	{
		public WeiXinController()
		{
		}

		[HttpPost]
		public JsonResult AddMenu(string title, string url, string parentId, int urlType)
		{
			short num;
			num = (short)(parentId != "0" ? 2 : 1);
			switch (urlType)
			{
				case 1:
				{
					string[] host = new string[] { "http://", base.HttpContext.Request.Url.Host, "/m-", PlatformType.WeiXin.ToString(), "/" };
					url = string.Concat(host);
					break;
				}
				case 2:
				{
					string[] strArrays = new string[] { "http://", base.HttpContext.Request.Url.Host, "/m-", PlatformType.WeiXin.ToString(), "/vshop/list" };
					url = string.Concat(strArrays);
					break;
				}
				case 3:
				{
					string[] host1 = new string[] { "http://", base.HttpContext.Request.Url.Host, "/m-", PlatformType.WeiXin.ToString(), "/category/Index" };
					url = string.Concat(host1);
					break;
				}
				case 4:
				{
					string[] strArrays1 = new string[] { "http://", base.HttpContext.Request.Url.Host, "/m-", PlatformType.WeiXin.ToString(), "/member/center" };
					url = string.Concat(strArrays1);
					break;
				}
				case 5:
				{
					string[] host2 = new string[] { "http://", base.HttpContext.Request.Url.Host, "/m-", PlatformType.WeiXin.ToString(), "/cart/cart" };
					url = string.Concat(host2);
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
				Depth = num,
				ShopId = base.CurrentManager.ShopId,
				FullIdPath = "1",
				Sequence = 1,
				UrlType = new MenuInfo.UrlTypes?((MenuInfo.UrlTypes)urlType)
			};
			ServiceHelper.Create<IWeixinMenuService>().AddMenu(menuInfo);
			result.success = true;
			return Json(result);
		}

		public JsonResult AddSlideImage(string id, string description, string imageUrl, string url)
		{
			Result result = new Result();
			SlideAdInfo slideAdInfo = new SlideAdInfo()
			{
				Id = Convert.ToInt64(id),
				ImageUrl = imageUrl,
				TypeId = SlideAdInfo.SlideAdType.WeixinHome,
				Url = url,
				Description = description,
				ShopId = 0
            };
			if (slideAdInfo.Id <= 0)
			{
				ServiceHelper.Create<ISlideAdsService>().AddSlidAd(slideAdInfo);
			}
			else
			{
				ServiceHelper.Create<ISlideAdsService>().UpdateSlidAd(slideAdInfo);
			}
			result.success = true;
			return Json(result);
		}

		public ActionResult BasicSettings()
		{
			SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
			if (string.IsNullOrEmpty(siteSettings.WeixinToken))
			{
				siteSettings.WeixinToken = CreateKey(8);
				ServiceHelper.Create<ISiteSettingService>().SetSiteSettings(siteSettings);
			}
			SiteSettingModel siteSettingModel = new SiteSettingModel()
			{
				WeixinAppId = siteSettings.WeixinAppId.Trim(),
				WeixinAppSecret = siteSettings.WeixinAppSecret.Trim(),
				WeixinToKen = siteSettings.WeixinToken.Trim()
			};
			SiteSettingModel siteSettingModel1 = siteSettingModel;
			ViewBag.Url = string.Format("http://{0}/m-Weixin/WXApi", base.Request.Url.Host);
			return View(siteSettingModel1);
		}

		[HttpPost]
		public JsonResult ChooseTopic(string frontCoverImage, long topicId)
		{
			ITopicService topicService = ServiceHelper.Create<ITopicService>();
			TopicInfo topicInfo = topicService.GetTopicInfo(topicId);
			topicInfo.FrontCoverImage = frontCoverImage;
			topicService.UpdateTopic(topicInfo);
			ServiceHelper.Create<IMobileHomeTopicService>().AddMobileHomeTopic(topicId, 0, PlatformType.WeiXin, frontCoverImage);
			return Json(new { success = true });
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
		public JsonResult DeleteSlideImage(string id)
		{
			Result result = new Result();
			ServiceHelper.Create<ISlideAdsService>().DeleteSlidAd(0, Convert.ToInt64(id));
			result.success = true;
			return Json(result);
		}

		public ActionResult EditMenu(long menuId)
		{
			MenuInfo menu = ServiceHelper.Create<IWeixinMenuService>().GetMenu(menuId);
			MenuManageModel menuManageModel = new MenuManageModel()
			{
				ID = menu.Id,
				TopMenuName = menu.Title,
				URL = menu.Url,
				LinkType = menu.UrlType
			};
			MenuManageModel menuManageModel1 = menuManageModel;
			if (menu.ParentId == 0)
			{
				ViewBag.parentId = 0;
			}
			else
			{
				ViewBag.parentName = ServiceHelper.Create<IWeixinMenuService>().GetMenu(menu.ParentId).Title;
				ViewBag.parentId = menu.ParentId;
			}
			return View(menuManageModel1);
		}

		public JsonResult GetSlideImages()
		{
			SlideAdInfo[] array = ServiceHelper.Create<ISlideAdsService>().GetSlidAds(0, SlideAdInfo.SlideAdType.WeixinHome).ToArray();
			ISlideAdsService slideAdsService = ServiceHelper.Create<ISlideAdsService>();
			var collection = array.Select((SlideAdInfo item) => {
				slideAdsService.GetSlidAd(0, item.Id);
				return new { id = item.Id, imgUrl = item.ImageUrl, displaySequence = item.DisplaySequence, url = item.Url, description = item.Description };
			});
			return Json(new { rows = collection, total = 100 }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult HomePageSetting()
		{
			MobileHomeTopicsInfo[] array = ServiceHelper.Create<IMobileHomeTopicService>().GetMobileHomeTopicInfos(PlatformType.WeiXin, 0).ToArray();
			ITopicService topicService = ServiceHelper.Create<ITopicService>();
			IEnumerable<TopicModel> topicModels = array.Select<MobileHomeTopicsInfo, TopicModel>((MobileHomeTopicsInfo item) => {
				TopicInfo topicInfo = topicService.GetTopicInfo(item.TopicId);
				return new TopicModel()
				{
					FrontCoverImage = topicInfo.FrontCoverImage,
					Id = item.Id,
					Name = topicInfo.Name,
					Tags = topicInfo.Tags,
					Sequence = item.Sequence
				};
			});
			ViewBag.Topic = topicModels;
			return View();
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult MenuManage()
		{
			List<MenuManageModel> menuManageModels = new List<MenuManageModel>();
			foreach (MenuInfo mainMenu in ServiceHelper.Create<IWeixinMenuService>().GetMainMenu(base.CurrentManager.ShopId))
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
			return View(menuManageModels);
		}

		public ActionResult ProductSettings()
		{
			return View();
		}

		[HttpPost]
		public JsonResult RemoveChoseTopic(long id)
		{
			ServiceHelper.Create<IMobileHomeTopicService>().Delete(id);
			return Json(new { success = true });
		}

		[HttpPost]
		public JsonResult RequestToWeixin()
		{
			Result result = new Result();
			ServiceHelper.Create<IWeixinMenuService>().ConsistentToWeixin(base.CurrentManager.ShopId);
			result.success = true;
			return Json(result);
		}

		public ActionResult SaveSlideImage(long id = 0L)
		{
			SlideAdInfo slideAdInfo;
			slideAdInfo = (id <= 0 ? new SlideAdInfo() : ServiceHelper.Create<ISlideAdsService>().GetSlidAd(0, id));
			SlideAdModel slideAdModel = new SlideAdModel()
			{
				Description = slideAdInfo.Description,
				imgUrl = slideAdInfo.ImageUrl,
				Url = slideAdInfo.Url,
				ID = id
			};
			return View(slideAdModel);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult SaveWeiXinSettings(string weixinAppId, string WeixinAppSecret)
		{
			Result result = new Result();
			SiteSettingsInfo siteSettings = ServiceHelper.Create<ISiteSettingService>().GetSiteSettings();
			siteSettings.WeixinAppId = weixinAppId.Trim();
			siteSettings.WeixinAppSecret = WeixinAppSecret.Trim();
			ServiceHelper.Create<ISiteSettingService>().SetSiteSettings(siteSettings);
			result.success = true;
			return Json(result);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult SlideImageChangeSequence(int oriRowNumber, int newRowNumber)
		{
			ServiceHelper.Create<ISlideAdsService>().UpdateWeixinSlideSequence(0, oriRowNumber, newRowNumber, SlideAdInfo.SlideAdType.WeixinHome);
			return Json(new { success = true });
		}

		public ActionResult SlideImageSettings()
		{
			SlideAdInfo[] array = ServiceHelper.Create<ISlideAdsService>().GetSlidAds(0, SlideAdInfo.SlideAdType.WeixinHome).ToArray();
			ISlideAdsService slideAdsService = ServiceHelper.Create<ISlideAdsService>();
			IEnumerable<SlideAdModel> slideAdModels = array.Select<SlideAdInfo, SlideAdModel>((SlideAdInfo item) => {
				slideAdsService.GetSlidAd(0, item.Id);
				return new SlideAdModel()
				{
					ID = item.Id,
					imgUrl = item.ImageUrl,
					DisplaySequence = item.DisplaySequence,
					Url = item.Url,
					Description = item.Description
				};
			});
			return View(slideAdModels);
		}

		public ActionResult TopicSettings()
		{
			MobileHomeTopicsInfo[] array = ServiceHelper.Create<IMobileHomeTopicService>().GetMobileHomeTopicInfos(PlatformType.WeiXin, 0).ToArray();
			ITopicService topicService = ServiceHelper.Create<ITopicService>();
			IEnumerable<TopicModel> topicModels = array.Select<MobileHomeTopicsInfo, TopicModel>((MobileHomeTopicsInfo item) => {
				TopicInfo topicInfo = topicService.GetTopicInfo(item.TopicId);
				return new TopicModel()
				{
					FrontCoverImage = topicInfo.FrontCoverImage,
					Id = item.Id,
					Name = topicInfo.Name,
					Tags = topicInfo.Tags,
					Sequence = item.Sequence
				};
			});
			return View(topicModels);
		}

		[HttpPost]
		public JsonResult UpdateMenu(string menuId, string menuName, int urlType, string url, string parentId)
		{
			switch (urlType)
			{
				case 1:
				{
					string[] host = new string[] { "http://", base.HttpContext.Request.Url.Host, "/m-", PlatformType.WeiXin.ToString(), "/" };
					url = string.Concat(host);
					break;
				}
				case 2:
				{
					string[] strArrays = new string[] { "http://", base.HttpContext.Request.Url.Host, "/m-", PlatformType.WeiXin.ToString(), "/vshop/list" };
					url = string.Concat(strArrays);
					break;
				}
				case 3:
				{
					string[] host1 = new string[] { "http://", base.HttpContext.Request.Url.Host, "/m-", PlatformType.WeiXin.ToString(), "/category/Index" };
					url = string.Concat(host1);
					break;
				}
				case 4:
				{
					string[] strArrays1 = new string[] { "http://", base.HttpContext.Request.Url.Host, "/m-", PlatformType.WeiXin.ToString(), "/member/center" };
					url = string.Concat(strArrays1);
					break;
				}
				case 5:
				{
					string[] host2 = new string[] { "http://", base.HttpContext.Request.Url.Host, "/m-", PlatformType.WeiXin.ToString(), "/cart/cart" };
					url = string.Concat(host2);
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
				Id = Convert.ToInt64(menuId),
				Title = menuName,
				UrlType = new MenuInfo.UrlTypes?((MenuInfo.UrlTypes)urlType),
				Url = url,
				ParentId = Convert.ToInt64(parentId)
			};
			ServiceHelper.Create<IWeixinMenuService>().UpdateMenu(menuInfo);
			result.success = true;
			return Json(result);
		}

		[HttpPost]
		public JsonResult UpdateSequence(long id, int sequence)
		{
			ServiceHelper.Create<IMobileHomeTopicService>().SetSequence(id, sequence, 0);
			return Json(new { success = true });
		}
	}
}