using Himall.Core;
using Himall.Core.Helper;
using Himall.IServices;
using Himall.IServices.QueryModel;
using Himall.Model;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Himall.Web.Areas.SellerAdmin.Controllers
{
	public class VShopController : BaseSellerController
	{
		public VShopController()
		{
		}

		public JsonResult BannerChangeSequence(int oriRowNumber, int newRowNumber)
		{
			ServiceHelper.Create<INavigationService>().SwapSellerDisplaySequence(base.CurrentSellerManager.ShopId, oriRowNumber, newRowNumber);
			return Json(new { success = true });
		}

		public JsonResult DeleteSlideImage(string id)
		{
			ServiceHelper.Create<ISlideAdsService>().DeleteSlidAd(base.CurrentSellerManager.ShopId, Convert.ToInt64(id));
			return Json(new { success = true });
		}

		public JsonResult DeleteVShopBanner(long id)
		{
			ServiceHelper.Create<INavigationService>().DeleteSellerformNavigation(base.CurrentSellerManager.ShopId, id);
			return Json(new { success = true });
		}

		public JsonResult EditSlideImage(SlideAdInfo slideAdInfo)
		{
			slideAdInfo.TypeId = SlideAdInfo.SlideAdType.VShopHome;
			slideAdInfo.ShopId = base.CurrentSellerManager.ShopId;
			if (slideAdInfo.Id <= 0)
			{
				ServiceHelper.Create<ISlideAdsService>().AddSlidAd(slideAdInfo);
			}
			else
			{
				ServiceHelper.Create<ISlideAdsService>().UpdateSlidAd(slideAdInfo);
			}
			return Json(new { success = true });
		}

		public ActionResult EditVShop(long shopId)
		{
			if (shopId != base.CurrentSellerManager.ShopId)
			{
				throw new HimallException("获取店铺信息错误");
			}
			VShopInfo vShopByShopId = ServiceHelper.Create<IVShopService>().GetVShopByShopId(shopId) ?? new VShopInfo();
			ViewBag.ShopId = base.CurrentSellerManager.ShopId;
			return View(vShopByShopId);
		}

		[HttpPost]
		public JsonResult EditVShop(VShopInfo vshopInfo)
		{
			if (vshopInfo.Id <= 0)
			{
				ServiceHelper.Create<IVShopService>().CreateVshop(vshopInfo);
			}
			else
			{
				ServiceHelper.Create<IVShopService>().UpdateVShop(vshopInfo);
			}
			return Json(new { success = true });
		}

		public JsonResult GetCouponList(int page, int rows, string couponName)
		{
			ICouponService couponService = ServiceHelper.Create<ICouponService>();
			CouponQuery couponQuery = new CouponQuery()
			{
				CouponName = couponName,
				ShopId = new long?(base.CurrentSellerManager.ShopId),
				PageSize = rows,
				PageNo = page
			};
			PageModel<CouponInfo> couponList = couponService.GetCouponList(couponQuery);
			IQueryable<long> vShopCouponSetting = 
				from item in ServiceHelper.Create<IVShopService>().GetVShopCouponSetting(base.CurrentSellerManager.ShopId)
				select item.CouponID;
			var array = 
				from item in couponList.Models.ToArray()
				select new { Id = item.Id, StartTime = item.StartTime.ToString("yyyy-MM-dd"), EndTime = item.EndTime.ToString("yyyy-MM-dd"), Price = Math.Round(item.Price, 2), CouponName = item.CouponName, PerMax = (item.PerMax == 0 ? "不限张" : string.Concat(item.PerMax.ToString(), "张/人")), OrderAmount = (item.OrderAmount == new decimal(0) ? "不限制" : string.Concat("满", item.OrderAmount)), IsSelect = vShopCouponSetting.Contains(item.Id) };
			return Json(new { rows = array, total = couponList.Total });
		}

		public JsonResult GetSlideImage()
		{
			IEnumerable<ImageAdInfo> imageAds = ServiceHelper.Create<ISlideAdsService>().GetImageAds(base.CurrentSellerManager.ShopId);
			var array = 
				from item in imageAds.ToArray()
                select new { id = item.Id, image = item.ImageUrl, url = item.Url };
			return Json(new { rows = array, total = 100 }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult GetSlideImage(long? id)
		{
			SlideAdInfo slideAdInfo;
			slideAdInfo = (!id.HasValue ? new SlideAdInfo() : ServiceHelper.Create<ISlideAdsService>().GetSlidAd(base.CurrentSellerManager.ShopId, id.Value));
			return Json(new { success = true, item = slideAdInfo });
		}

		public JsonResult GetSlideImages()
		{
			SlideAdInfo[] array = ServiceHelper.Create<ISlideAdsService>().GetSlidAds(base.CurrentSellerManager.ShopId, SlideAdInfo.SlideAdType.VShopHome).ToArray();
			ISlideAdsService slideAdsService = ServiceHelper.Create<ISlideAdsService>();
			var collection = array.Select((SlideAdInfo item) => {
				slideAdsService.GetSlidAd(CurrentSellerManager.ShopId, item.Id);
				return new { id = item.Id, image = item.ImageUrl, displaySequence = (item.DisplaySequence == 0 ? "0" : item.DisplaySequence.ToString()), url = item.Url };
			});
			return Json(new { rows = collection, total = 100 }, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetVShopBanner(long? id)
		{
			BannerInfo bannerInfo = new BannerInfo();
			if (id.HasValue)
			{
				bannerInfo = ServiceHelper.Create<INavigationService>().GetSellerNavigation(id.Value);
			}
			return Json(new { success = true, item = bannerInfo });
		}

		public JsonResult GetVShopBanners()
		{
			BannerInfo[] array = ServiceHelper.Create<INavigationService>().GetSellerNavigations(base.CurrentSellerManager.ShopId, PlatformType.WeiXin).ToArray();
			var variable = 
				from item in array
                select new { id = item.Id, name = item.Name, url = string.Concat(item.UrlType.ToDescription(), ' ', item.Url), displaySequence = item.DisplaySequence };
			return Json(new { rows = variable, tota = 100 }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Management()
		{
			Image image;
			VShopInfo vShopByShopId = ServiceHelper.Create<IVShopService>().GetVShopByShopId(base.CurrentSellerManager.ShopId);
			string empty = string.Empty;
			if (vShopByShopId != null)
			{
				object[] host = new object[] { "http://", base.HttpContext.Request.Url.Host, "/m-", PlatformType.WeiXin.ToString(), "/vshop/detail/", vShopByShopId.Id };
				string str = string.Concat(host);
				string str1 = Server.MapPath(vShopByShopId.Logo);
				image = (string.IsNullOrWhiteSpace(vShopByShopId.Logo) || !System.IO.File.Exists(str1) ? QRCodeHelper.Create(str) : QRCodeHelper.Create(str, str1));
				DateTime now = DateTime.Now;
				string str2 = string.Concat(now.ToString("yyMMddHHmmssffffff"), ".jpg");
				empty = string.Concat("/temp/", str2);
				image.Save(string.Concat(Server.MapPath("~/temp/"), str2));
				image.Dispose();
			}
			ViewBag.QRCode = empty;
			ViewBag.ShopId = base.CurrentSellerManager.ShopId;
			return View(vShopByShopId);
		}

		public JsonResult SaveGouponSetting(string ids, string values)
		{
			string[] strArrays = values.Split(new char[] { ',' });
			string[] strArrays1 = ids.Split(new char[] { ',' });
			List<CouponSettingInfo> couponSettingInfos = new List<CouponSettingInfo>();
			for (int i = 0; i < strArrays.Length; i++)
			{
				List<CouponSettingInfo> couponSettingInfos1 = couponSettingInfos;
				CouponSettingInfo couponSettingInfo = new CouponSettingInfo()
				{
					Display = new int?((string.IsNullOrEmpty(strArrays[i]) ? 0 : int.Parse(strArrays[i]))),
					CouponID = long.Parse(strArrays1[i]),
					PlatForm = PlatformType.Mobile
				};
				couponSettingInfos1.Add(couponSettingInfo);
			}
			ServiceHelper.Create<IVShopService>().SaveVShopCouponSetting(couponSettingInfos);
			return Json(new { success = true }, JsonRequestBehavior.AllowGet);
		}

		public JsonResult SaveSlideImage(long? id, string imageUrl, string url)
		{
			SlideAdInfo slideAdInfo = new SlideAdInfo()
			{
				ImageUrl = imageUrl,
				Url = url,
				ShopId = base.CurrentSellerManager.ShopId,
				TypeId = SlideAdInfo.SlideAdType.VShopHome
			};
			if (!id.HasValue)
			{
				ServiceHelper.Create<ISlideAdsService>().AddSlidAd(slideAdInfo);
			}
			else
			{
				slideAdInfo.Id = id.Value;
				ServiceHelper.Create<ISlideAdsService>().UpdateSlidAd(slideAdInfo);
			}
			return Json(new { success = true });
		}

		public JsonResult SaveVShopBanner(long? id, string bannerName, string url, int urlType)
		{
			VShopInfo vShopByShopId = ServiceHelper.Create<IVShopService>().GetVShopByShopId(base.CurrentSellerManager.ShopId);
			switch (urlType)
			{
				case 1:
				{
					object[] str = new object[] { "/m-", PlatformType.WeiXin.ToString(), "/vshop/Search?vshopid=", vShopByShopId.Id };
					url = string.Concat(str);
					break;
				}
				case 2:
				{
					object[] objArray = new object[] { "/m-", PlatformType.WeiXin.ToString(), "/vshop/Category?vshopid=", vShopByShopId.Id };
					url = string.Concat(objArray);
					break;
				}
				case 3:
				{
					object[] str1 = new object[] { "/m-", PlatformType.WeiXin.ToString(), "/vshop/introduce/", vShopByShopId.Id };
					url = string.Concat(str1);
					break;
				}
			}
			BannerInfo bannerInfo = new BannerInfo()
			{
				Name = bannerName,
				Platform = PlatformType.WeiXin,
				ShopId = base.CurrentSellerManager.ShopId,
				Url = url,
				Position = 0,
				UrlType = (BannerInfo.BannerUrltypes)urlType
			};
			if (!id.HasValue)
			{
				ServiceHelper.Create<INavigationService>().AddSellerNavigation(bannerInfo);
			}
			else
			{
				bannerInfo.Id = id.Value;
				ServiceHelper.Create<INavigationService>().UpdateSellerNavigation(bannerInfo);
			}
			return Json(new { success = true, item = bannerInfo });
		}

		public JsonResult SaveVShopHomePageTitle(string homePageTitle)
		{
			VShopInfo vShopByShopId = ServiceHelper.Create<IVShopService>().GetVShopByShopId(base.CurrentSellerManager.ShopId);
			vShopByShopId.HomePageTitle = homePageTitle;
			ServiceHelper.Create<IVShopService>().UpdateVShop(vShopByShopId);
			return Json(new { success = true });
		}

		[HttpPost]
		public JsonResult SlideImageChangeSequence(int oriRowNumber, int newRowNumber)
		{
			ServiceHelper.Create<ISlideAdsService>().UpdateWeixinSlideSequence(base.CurrentSellerManager.ShopId, oriRowNumber, newRowNumber, SlideAdInfo.SlideAdType.VShopHome);
			return Json(new { success = true });
		}

		public ActionResult VshopHomeSite()
		{
			VShopInfo vShopByShopId = ServiceHelper.Create<IVShopService>().GetVShopByShopId(base.CurrentSellerManager.ShopId);
			ViewBag.VShop = vShopByShopId;
			ViewBag.ShopId = base.CurrentSellerManager.ShopId;
			ViewBag.SlideImage = ServiceHelper.Create<ISlideAdsService>().GetSlidAds(base.CurrentSellerManager.ShopId, SlideAdInfo.SlideAdType.VShopHome);
			ViewBag.Banner = ServiceHelper.Create<INavigationService>().GetSellerNavigations(base.CurrentSellerManager.ShopId, PlatformType.WeiXin);
			return View(vShopByShopId);
		}
	}
}