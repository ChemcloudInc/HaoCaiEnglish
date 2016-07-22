using Himall.Core.Helper;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Framework;
using Himall.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.Admin.Controllers
{
	public class SlideAdController : BaseAdminController
	{
		public SlideAdController()
		{
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult AddHandSlideAd(string pic, string url)
		{
			HandSlideAdInfo handSlideAdInfo = new HandSlideAdInfo()
			{
				ImageUrl = pic,
				Url = url,
				DisplaySequence = 0
            };
			HandSlideAdInfo handSlideAdInfo1 = handSlideAdInfo;
			if (!string.IsNullOrWhiteSpace(pic))
			{
				string str = Server.MapPath(pic);
				string str1 = "/Storage/Plat/ImageAd/";
				string mapPath = IOHelper.GetMapPath(str1);
				if (!Directory.Exists(mapPath))
				{
					Directory.CreateDirectory(mapPath);
				}
				IOHelper.CopyFile(str, Server.MapPath(str1), true, "");
				handSlideAdInfo1.ImageUrl = Path.Combine(str1, Path.GetFileName(str));
			}
			ServiceHelper.Create<ISlideAdsService>().AddHandSlidAd(handSlideAdInfo1);
			return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[UnAuthorize]
		public JsonResult AddSlideAd(string pic, string url)
		{
			SlideAdInfo slideAdInfo = new SlideAdInfo()
			{
				ImageUrl = pic,
				Url = url,
				ShopId = 0,
				DisplaySequence = 0,
				TypeId = SlideAdInfo.SlideAdType.PlatformHome
			};
			SlideAdInfo slideAdInfo1 = slideAdInfo;
			if (!string.IsNullOrWhiteSpace(pic))
			{
				string str = Server.MapPath(pic);
				string str1 = "/Storage/Plat/ImageAd/";
				string mapPath = IOHelper.GetMapPath(str1);
				if (!Directory.Exists(mapPath))
				{
					Directory.CreateDirectory(mapPath);
				}
				IOHelper.CopyFile(str, Server.MapPath(str1), true, "");
				slideAdInfo1.ImageUrl = Path.Combine(str1, Path.GetFileName(str));
			}
			ServiceHelper.Create<ISlideAdsService>().AddSlidAd(slideAdInfo1);
			return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[UnAuthorize]
		public ActionResult AdjustHandSlideIndex(long id, int direction)
		{
			ServiceHelper.Create<ISlideAdsService>().AdjustHandSlidAdIndex(id, direction == 1);
			return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[UnAuthorize]
		public ActionResult AdjustSlideIndex(long id, int direction)
		{
			ServiceHelper.Create<ISlideAdsService>().AdjustSlidAdIndex(0, id, direction == 1, SlideAdInfo.SlideAdType.PlatformHome);
			return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
		}

		[UnAuthorize]
		public JsonResult DeleteHandSlide(long Id)
		{
			ServiceHelper.Create<ISlideAdsService>().DeleteHandSlidAd(Id);
			return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
		}

		[UnAuthorize]
		public JsonResult DeleteSlide(long Id)
		{
			ServiceHelper.Create<ISlideAdsService>().DeleteSlidAd(0, Id);
			return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
		}

		[UnAuthorize]
		public JsonResult EditHandSlideAd(long id, string pic, string url)
		{
			HandSlideAdInfo handSlidAd = ServiceHelper.Create<ISlideAdsService>().GetHandSlidAd(id);
			if (!string.IsNullOrWhiteSpace(pic) && !handSlidAd.ImageUrl.Equals(pic))
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
			ISlideAdsService slideAdsService = ServiceHelper.Create<ISlideAdsService>();
			HandSlideAdInfo handSlideAdInfo = new HandSlideAdInfo()
			{
				Id = id,
				ImageUrl = pic,
				Url = url
			};
			slideAdsService.UpdateHandSlidAd(handSlideAdInfo);
			return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
		}

		[UnAuthorize]
		public JsonResult EditSlideAd(long id, string pic, string url)
		{
			SlideAdInfo slidAd = ServiceHelper.Create<ISlideAdsService>().GetSlidAd(0, id);
			if (!string.IsNullOrWhiteSpace(pic) && !slidAd.ImageUrl.Equals(pic))
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
			ISlideAdsService slideAdsService = ServiceHelper.Create<ISlideAdsService>();
			SlideAdInfo slideAdInfo = new SlideAdInfo()
			{
				Id = id,
				ImageUrl = pic,
				Url = url
			};
			slideAdsService.UpdateSlidAd(slideAdInfo);
			return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
		}

		[UnAuthorize]
		public JsonResult GetHandSlideJson()
		{
			IEnumerable<HandSlideModel> array = 
				from item in ServiceHelper.Create<ISlideAdsService>().GetHandSlidAds().ToArray()
                select new HandSlideModel()
				{
					Id = item.Id,
					Pic = item.ImageUrl,
					URL = item.Url,
					Index = item.DisplaySequence
				};
			DataGridModel<HandSlideModel> dataGridModel = new DataGridModel<HandSlideModel>()
			{
				rows = array,
				total = array.Count()
			};
			return Json(dataGridModel);
		}

		[UnAuthorize]
		public JsonResult GetSlideJson()
		{
			IQueryable<SlideAdInfo> slidAds = ServiceHelper.Create<ISlideAdsService>().GetSlidAds(0, SlideAdInfo.SlideAdType.PlatformHome);
			IEnumerable<HandSlideModel> array = 
				from item in slidAds.ToArray()
                select new HandSlideModel()
				{
					Id = item.Id,
					Pic = item.ImageUrl,
					URL = item.Url,
					Index = item.DisplaySequence
				};
			DataGridModel<HandSlideModel> dataGridModel = new DataGridModel<HandSlideModel>()
			{
				rows = array,
				total = array.Count()
			};
			return Json(dataGridModel);
		}

		public ActionResult HandSlideManagement()
		{
			return View();
		}

		public ActionResult SlideManagement()
		{
			return View();
		}

		[HttpPost]
		[UnAuthorize]
		public ActionResult UpdateImageAd(long id, string pic, string url)
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
			return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
		}
	}
}