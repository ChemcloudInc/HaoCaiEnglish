using Himall.Core;
using Himall.Core.Plugins;
using Himall.Core.Plugins.OAuth;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Web.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class HomeController : BaseController
	{
		public HomeController()
		{
		}

		[HttpGet]
		public JsonResult GetFoot()
		{
			IArticleCategoryService articleCategoryService = ServiceHelper.Create<IArticleCategoryService>();
			IArticleService articleService = ServiceHelper.Create<IArticleService>();
			ArticleCategoryInfo specialArticleCategory = articleCategoryService.GetSpecialArticleCategory(SpecialCategory.PageFootService);
			if (specialArticleCategory == null)
			{
				return Json(new List<PageFootServiceModel>(), JsonRequestBehavior.AllowGet);
			}
			IQueryable<ArticleCategoryInfo> articleCategoriesByParentId = articleCategoryService.GetArticleCategoriesByParentId(specialArticleCategory.Id, false);
			IEnumerable<PageFootServiceModel> array = 
				from item in articleCategoriesByParentId.ToArray()
				select new PageFootServiceModel()
				{
					CateogryName = item.Name,
					Articles = 
						from t in articleService.GetArticleByArticleCategoryId(item.Id)
						where t.IsRelease
						select t
				};
			return Json(array, JsonRequestBehavior.AllowGet);
		}

		private IEnumerable<string> GetOAuthValidateContents()
		{
			return 
				from item in PluginsManagement.GetPlugins<IOAuthPlugin>(true)
				select item.Biz.GetValidateContent();
		}

		[HttpGet]
		public ActionResult Index()
		{
			string str;
			if (!IsInstalled())
			{
				return RedirectToAction("Agreement", "Installer");
			}
			ServiceHelper.Create<IMemberService>();
			ViewBag.OAuthValidateContents = GetOAuthValidateContents();
			ViewBag.SiteName = base.CurrentSiteSetting.SiteName;
			dynamic viewBag = base.ViewBag;
			str = (string.IsNullOrWhiteSpace(base.CurrentSiteSetting.Site_SEOTitle) ? "商城首页" : base.CurrentSiteSetting.Site_SEOTitle);
			viewBag.Title = str;
			ViewEngines.Engines.FindView(base.ControllerContext, "Index", null);
			List<HomeFloorModel> homeFloorModels = new List<HomeFloorModel>();
			ViewBag.handImage = ServiceHelper.Create<ISlideAdsService>().GetHandSlidAds().ToList();
			List<SlideAdInfo> list = ServiceHelper.Create<ISlideAdsService>().GetSlidAds(0, SlideAdInfo.SlideAdType.PlatformHome).ToList();
			ViewBag.slideImage = list;
			dynamic obj = base.ViewBag;
			IEnumerable<ImageAdInfo> imageAds = ServiceHelper.Create<ISlideAdsService>().GetImageAds(0);
			obj.imageAds = imageAds.Where((ImageAdInfo p) => {
				if (p.Id <= 0)
				{
					return false;
				}
				return p.Id <= 8;
			}).ToList();
			dynamic viewBag1 = base.ViewBag;
			IEnumerable<ImageAdInfo> imageAdInfos = ServiceHelper.Create<ISlideAdsService>().GetImageAds(0);
			viewBag1.imageAdsTop = imageAdInfos.Where((ImageAdInfo p) => {
				if (p.Id == 14)
				{
					return true;
				}
				return p.Id == 15;
			}).ToList();
		    IArticleService articleService = ServiceHelper.Create<IArticleService>();
			dynamic obj1 = base.ViewBag;
			List<IQueryable<ArticleInfo>> queryables = new List<IQueryable<ArticleInfo>>()
			{
				articleService.GetTopNArticle<ArticleInfo>(8, 4, null, false),
				articleService.GetTopNArticle<ArticleInfo>(8, 5, null, false),
				articleService.GetTopNArticle<ArticleInfo>(8, 6, null, false),
				articleService.GetTopNArticle<ArticleInfo>(8, 7, null, false)
			};
			obj1.ArticleTabs = queryables;
			foreach (HomeFloorInfo homeFloorInfo in ServiceHelper.Create<IFloorService>().GetHomeFloors().ToList())    //超链接
			{
				HomeFloorModel homeFloorModel = new HomeFloorModel();
				List<FloorTopicInfo> floorTopicInfos = (
					from a in homeFloorInfo.FloorTopicInfo
					where a.TopicType == Position.Top
					select a).ToList();
				List<FloorTopicInfo> list1 = (
					from a in homeFloorInfo.FloorTopicInfo
					where a.TopicType != Position.Top
					select a).ToList();
				List<FloorBrandInfo> floorBrandInfos = homeFloorInfo.FloorBrandInfo.Take(10).ToList();
				homeFloorModel.Name = homeFloorInfo.FloorName;
				homeFloorModel.SubName = homeFloorInfo.SubName;
				homeFloorModel.StyleLevel = homeFloorInfo.StyleLevel;
				homeFloorModel.DefaultTabName = homeFloorInfo.DefaultTabName;
				foreach (FloorTopicInfo floorTopicInfo in floorTopicInfos)
				{
					List<HomeFloorModel.WebFloorTextLink> textLinks = homeFloorModel.TextLinks;
					HomeFloorModel.WebFloorTextLink webFloorTextLink = new HomeFloorModel.WebFloorTextLink()
					{
						Id = floorTopicInfo.Id,
						Name = floorTopicInfo.TopicName,
						Url = floorTopicInfo.Url
					};
					textLinks.Add(webFloorTextLink);
				}
				foreach (FloorTopicInfo floorTopicInfo1 in list1)
				{
					List<HomeFloorModel.WebFloorProductLinks> products = homeFloorModel.Products;
					HomeFloorModel.WebFloorProductLinks webFloorProductLink = new HomeFloorModel.WebFloorProductLinks()
					{
						Id = floorTopicInfo1.Id,
						ImageUrl = floorTopicInfo1.TopicImage,
						Url = floorTopicInfo1.Url,
						Type = floorTopicInfo1.TopicType
					};
					products.Add(webFloorProductLink);
				}
				foreach (FloorBrandInfo floorBrandInfo in floorBrandInfos)
				{
					List<HomeFloorModel.WebFloorBrand> brands = homeFloorModel.Brands;
					HomeFloorModel.WebFloorBrand webFloorBrand = new HomeFloorModel.WebFloorBrand()
					{
						Id = floorBrandInfo.BrandInfo.Id,
						Img = floorBrandInfo.BrandInfo.Logo,
						Url = "",
						Name = floorBrandInfo.BrandInfo.Name
					};
					brands.Add(webFloorBrand);
				}
				if (homeFloorModel.StyleLevel == 1)
				{
					homeFloorModel.Tabs = (
						from p in homeFloorInfo.Himall_FloorTabls
						orderby p.Id
						select new HomeFloorModel.Tab()
						{
							Name = p.Name,
							Detail = (
								from d in p.Himall_FloorTablDetails
								select new HomeFloorModel.ProductDetail()
								{
									ProductId = d.Himall_Products.Id,
									ImagePath = d.Himall_Products.ImagePath,
									Price = d.Himall_Products.MinSalePrice,
									Name = d.Himall_Products.ProductName
								}).ToList<HomeFloorModel.ProductDetail>()
						}).ToList<HomeFloorModel.Tab>();
					homeFloorModel.Scrolls = homeFloorModel.Products.Where<HomeFloorModel.WebFloorProductLinks>((HomeFloorModel.WebFloorProductLinks p) => {
						if (p.Type == Position.ScrollOne || p.Type == Position.ScrollTwo || p.Type == Position.ScrollThree)
						{
							return true;
						}
						return p.Type == Position.ScrollFour;
					}).ToList<HomeFloorModel.WebFloorProductLinks>();
					homeFloorModel.RightTops = homeFloorModel.Products.Where<HomeFloorModel.WebFloorProductLinks>((HomeFloorModel.WebFloorProductLinks p) => {
						if (p.Type == Position.ROne || p.Type == Position.RTwo || p.Type == Position.RThree)
						{
							return true;
						}
						return p.Type == Position.RFour;
					}).ToList<HomeFloorModel.WebFloorProductLinks>();
					homeFloorModel.RightBottons = homeFloorModel.Products.Where<HomeFloorModel.WebFloorProductLinks>((HomeFloorModel.WebFloorProductLinks p) => {
						if (p.Type == Position.RFive || p.Type == Position.RSix || p.Type == Position.RSeven)
						{
							return true;
						}
						return p.Type == Position.REight;
					}).ToList<HomeFloorModel.WebFloorProductLinks>();
				}
				homeFloorModels.Add(homeFloorModel);
			}
			return View(homeFloorModels);
		}

		[HttpHead]
		public ContentResult Index(string s)
		{
			return base.Content("");
		}

		public ActionResult Index2()
		{
			return View();
		}

		private bool IsInstalled()
		{
			string item = ConfigurationManager.AppSettings["IsInstalled"];
			if (item == null)
			{
				return true;
			}
			return bool.Parse(item);
		}

		public ActionResult TestLogin()
		{
			return View();
		}
	}
}