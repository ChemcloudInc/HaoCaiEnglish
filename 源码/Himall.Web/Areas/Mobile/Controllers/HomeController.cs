using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Mobile.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Mobile.Controllers
{
	public class HomeController : BaseMobileTemplatesController
	{
		public HomeController()
		{
		}

		public ActionResult About()
		{
			return View();
		}

		public ActionResult Index()
		{
			SlideAdInfo.SlideAdType slideAdType;
			slideAdType = (base.PlatformType != Himall.Core.PlatformType.WeiXin ? SlideAdInfo.SlideAdType.WeixinHome : SlideAdInfo.SlideAdType.WeixinHome);
			Himall.Core.PlatformType platformType = Himall.Core.PlatformType.WeiXin;
			IQueryable<SlideAdInfo> slidAds = ServiceHelper.Create<ISlideAdsService>().GetSlidAds(0, slideAdType);
			dynamic viewBag = base.ViewBag;
			SlideAdInfo[] array = slidAds.ToArray();
			viewBag.SlideAds = 
				from item in array
                select new HomeSlideAdsModel()
				{
					ImageUrl = item.ImageUrl,
					Url = item.Url
				};
			MobileHomeTopicsInfo[] mobileHomeTopicsInfoArray = (
				from item in ServiceHelper.Create<IMobileHomeTopicService>().GetMobileHomeTopicInfos(platformType, 0)
				orderby item.Sequence
				select item).ToArray();
			ITopicService topicService = ServiceHelper.Create<ITopicService>();
			IEnumerable<HomeTopicModel> homeTopicModels = mobileHomeTopicsInfoArray.Select<MobileHomeTopicsInfo, HomeTopicModel>((MobileHomeTopicsInfo item) => {
				TopicInfo topicInfo = topicService.GetTopicInfo(item.TopicId);
				string[] strArrays = topicInfo.Tags.Split(new char[] { ',' });
				return new HomeTopicModel()
				{
					ImageUrl = topicInfo.FrontCoverImage,
					Tags = strArrays,
					Id = item.TopicId
				};
			});
			ViewBag.Topics = homeTopicModels;
			IQueryable<MobileHomeProductsInfo> mobileHomeProductsInfos = (
				from item in ServiceHelper.Create<IMobileHomeProductsService>().GetMobileHomePageProducts(0, platformType)
				orderby item.Sequence, item.Id descending
				select item).Take(8);
			IEnumerable<ProductItem> productItems = 
				from item in mobileHomeProductsInfos.ToArray()
                select new ProductItem()
				{
					Id = item.ProductId,
					ImageUrl = item.Himall_Products.GetImage(ProductInfo.ImageSize.Size_350, 1),
					Name = item.Himall_Products.ProductName,
					MarketPrice = item.Himall_Products.MarketPrice,
					SalePrice = item.Himall_Products.MinSalePrice
				};
			ViewBag.Products = productItems;
			ViewBag.SiteName = base.CurrentSiteSetting.SiteName;
			return View();
		}

		public JsonResult LoadProducts(int page, int pageSize)
		{
			IQueryable<MobileHomeProductsInfo> mobileHomeProductsInfos = (
				from item in ServiceHelper.Create<IMobileHomeProductsService>().GetMobileHomePageProducts(0, Himall.Core.PlatformType.WeiXin)
				orderby item.Sequence, item.Id descending
				select item).Skip((page - 1) * pageSize).Take(pageSize);
			var array = 
				from item in mobileHomeProductsInfos.ToArray()
                select new { name = item.Himall_Products.ProductName, id = item.ProductId, image = item.Himall_Products.GetImage(ProductInfo.ImageSize.Size_350, 1), price = item.Himall_Products.MinSalePrice, marketPrice = item.Himall_Products.MarketPrice };
			return Json(array, JsonRequestBehavior.AllowGet);
		}
	}
}