using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class PageSettingsController : BaseSellerController
	{
		public PageSettingsController()
		{
		}

		[HttpPost]
		[UnAuthorize]
		public ActionResult AdjustSlideIndex(long id, int direction)
		{
			ServiceHelper.Create<ISlideAdsService>().AdjustSlidAdIndex(base.CurrentSellerManager.ShopId, id, direction == 1, SlideAdInfo.SlideAdType.ShopHome);
			return Json(new { success = true });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult DeleteSlide(long Id)
		{
			ServiceHelper.Create<ISlideAdsService>().DeleteSlidAd(base.CurrentSellerManager.ShopId, Id);
			return Json(new { success = true });
		}

		public ActionResult Management()
		{
			IOrderedEnumerable<ImageAdInfo> imageAds = 
				from item in ServiceHelper.Create<ISlideAdsService>().GetImageAds(base.CurrentSellerManager.ShopId)
				orderby item.Id
				select item;
			ViewBag.Logo = ServiceHelper.Create<IShopService>().GetShop(base.CurrentSellerManager.ShopId, false).Logo;
			return View(imageAds);
		}

		private string MoveImages(string image)
		{
			string mapPath = IOHelper.GetMapPath(image);
			string extension = (new FileInfo(mapPath)).Extension;
			string empty = string.Empty;
			string str = string.Format("/Storage/Shop/{0}/ImageAd", base.CurrentSellerManager.ShopId);
			empty = IOHelper.GetMapPath(str);
			if (!Directory.Exists(empty))
			{
				Directory.CreateDirectory(empty);
			}
			if (image.Replace("\\", "/").Contains("/temp/"))
			{
				IOHelper.CopyFile(mapPath, empty, true, string.Concat("logo", extension));
			}
			return string.Concat(str, "/logo", extension);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult SaveSlideAd(long? id, string pic, string url)
		{
			SlideAdInfo slideAdInfo = new SlideAdInfo()
			{
				ImageUrl = pic,
				Url = url,
				ShopId = base.CurrentSellerManager.ShopId,
				DisplaySequence = 0,
				Id = id.GetValueOrDefault(),
				TypeId = SlideAdInfo.SlideAdType.ShopHome
			};
			SlideAdInfo slideAdInfo1 = slideAdInfo;
			if (!string.IsNullOrWhiteSpace(pic) && pic.Contains("/temp/"))
			{
				string str = Server.MapPath(pic);
				string str1 = string.Format("/Storage/Shop/{0}/ImageAd/", base.CurrentSellerManager.ShopId);
				string str2 = Server.MapPath(str1);
				if (!Directory.Exists(str2))
				{
					Directory.CreateDirectory(str2);
				}
				IOHelper.CopyFile(str, Server.MapPath(str1), true, "");
				slideAdInfo1.ImageUrl = Path.Combine(str1, Path.GetFileName(str));
			}
			if (!id.HasValue)
			{
				ServiceHelper.Create<ISlideAdsService>().AddSlidAd(slideAdInfo1);
			}
			else
			{
				ServiceHelper.Create<ISlideAdsService>().UpdateSlidAd(slideAdInfo1);
			}
			return Json(new { success = true, imageUrl = slideAdInfo1.ImageUrl });
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult SetLogo(string logo)
		{
			string str = MoveImages(logo);
			ServiceHelper.Create<IShopService>().UpdateLogo(base.CurrentSellerManager.ShopId, str);
			return Json(new { success = true, logo = str });
		}

		public ActionResult SlideAds()
		{
			IQueryable<SlideAdInfo> slidAds = ServiceHelper.Create<ISlideAdsService>().GetSlidAds(base.CurrentSellerManager.ShopId, SlideAdInfo.SlideAdType.ShopHome);
			return View(slidAds);
		}

		[HttpPost]
		[UnAuthorize]
		public ActionResult UpdateImageAd(long id, string pic, string url)
		{
			ImageAdInfo imageAd = ServiceHelper.Create<ISlideAdsService>().GetImageAd(base.CurrentSellerManager.ShopId, id);
			if (!string.IsNullOrWhiteSpace(pic) && !imageAd.ImageUrl.Equals(pic))
			{
				string str = Server.MapPath(pic);
				string str1 = string.Format("/Storage/Shop/{0}/ImageAd/", base.CurrentSellerManager.ShopId);
				string str2 = Server.MapPath(str1);
				if (!Directory.Exists(str2))
				{
					Directory.CreateDirectory(str2);
				}
				IOHelper.CopyFile(str, str2, true, "");
				pic = Path.Combine(str1, Path.GetFileName(str));
			}
			ImageAdInfo imageAdInfo = new ImageAdInfo()
			{
				ShopId = base.CurrentSellerManager.ShopId,
				Url = url,
				ImageUrl = pic,
				Id = id
			};
			ServiceHelper.Create<ISlideAdsService>().UpdateImageAd(imageAdInfo);
			return Json(new { success = true, imageUrl = pic });
		}
	}
}