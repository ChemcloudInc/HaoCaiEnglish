using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Admin.Models;
using Himall.Web.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class APPShopController : BaseAdminController
	{
		public APPShopController()
		{
		}

		public JsonResult AddSlideImage(string id, string description, string imageUrl, string url)
		{
			Result result = new Result();
			SlideAdInfo slideAdInfo = new SlideAdInfo()
			{
				Id = Convert.ToInt64(id),
				ImageUrl = imageUrl,
				TypeId = SlideAdInfo.SlideAdType.IOSShopHome,
				Url = url.ToLower().Replace("/m-wap", "/m-ios").Replace("/m-weixin", "/m-ios"),
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

		[HttpPost]
		public JsonResult DeleteSlideImage(string id)
		{
			Result result = new Result();
			ServiceHelper.Create<ISlideAdsService>().DeleteSlidAd(0, Convert.ToInt64(id));
			result.success = true;
			return Json(result);
		}

		public JsonResult GetImageAd(long id)
		{
			ImageAdInfo imageAd = ServiceHelper.Create<ISlideAdsService>().GetImageAd(0, id);
			return Json(new { success = true, imageUrl = imageAd.ImageUrl, url = imageAd.Url }, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetSlideImages()
		{
			SlideAdInfo[] array = ServiceHelper.Create<ISlideAdsService>().GetSlidAds(0, SlideAdInfo.SlideAdType.IOSShopHome).ToArray();
			ISlideAdsService slideAdsService = ServiceHelper.Create<ISlideAdsService>();
			var collection = array.Select((SlideAdInfo item) => {
				slideAdsService.GetSlidAd(0, item.Id);
				return new { id = item.Id, imgUrl = item.ImageUrl, displaySequence = item.DisplaySequence, url = item.Url, description = item.Description };
			});
			return Json(new { rows = collection, total = 100 }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult HomePageSetting()
		{
			MobileHomeTopicsInfo[] array = ServiceHelper.Create<IMobileHomeTopicService>().GetMobileHomeTopicInfos(PlatformType.IOS, 0).ToArray();
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
		[UnAuthorize]
		public JsonResult SlideImageChangeSequence(int oriRowNumber, int newRowNumber)
		{
			ServiceHelper.Create<ISlideAdsService>().UpdateWeixinSlideSequence(0, oriRowNumber, newRowNumber, SlideAdInfo.SlideAdType.IOSShopHome);
			return Json(new { success = true });
		}

		public JsonResult UpdateImageAd(long id, string pic, string url)
		{
			ImageAdInfo imageAd = ServiceHelper.Create<ISlideAdsService>().GetImageAd(0, id);
			if (!string.IsNullOrWhiteSpace(pic) && !imageAd.ImageUrl.Equals(pic))
			{
				string str = Server.MapPath(pic);
				string str1 = "/Storage/Plat/ImageAd/";
				string mapPath = IOHelper.GetMapPath(str1);
				if (!Directory.Exists(mapPath))
				{
					Directory.CreateDirectory(mapPath);
				}
				IOHelper.CopyFile(str, Server.MapPath(str1), true, "");
				pic = Path.Combine(str1, Path.GetFileName(str));
			}
			ImageAdInfo imageAdInfo = new ImageAdInfo()
			{
				ShopId = 0,
				Url = url,
				ImageUrl = pic,
				Id = id
			};
			ServiceHelper.Create<ISlideAdsService>().UpdateImageAd(imageAdInfo);
			return Json(new { success = true });
		}
	}
}