using Himall.Core;
using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Web;
using Himall.Web.Areas.Web.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class ProductPartialController : BaseWebController
	{
		public ProductPartialController()
		{
		}

		public ActionResult Foot()
		{
			IArticleCategoryService articleCategoryService = ServiceHelper.Create<IArticleCategoryService>();
			IArticleService articleService = ServiceHelper.Create<IArticleService>();
			ArticleCategoryInfo specialArticleCategory = articleCategoryService.GetSpecialArticleCategory(SpecialCategory.PageFootService);
			if (specialArticleCategory == null)
			{
				return base.PartialView("~/Areas/Web/Views/Shared/Foot.cshtml");
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
			ViewBag.PageFootService = array;
			ViewBag.PageFoot = base.CurrentSiteSetting.PageFoot;
			ViewBag.QRCode = base.CurrentSiteSetting.QRCode;
			ViewBag.SiteName = base.CurrentSiteSetting.SiteName;
			return base.PartialView("~/Areas/Web/Views/Shared/Foot.cshtml");
		}

		[HttpGet]
		public JsonResult GetBrowedProduct()
		{
			return Json(BrowseHistrory.GetBrowsingProducts(5, (base.CurrentUser == null ? 0 : base.CurrentUser.Id)), JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetBrowedProductList()
		{
			return PartialView("_ProductBrowsedHistory", BrowseHistrory.GetBrowsingProducts(5, (base.CurrentUser == null ? 0 : base.CurrentUser.Id)));
		}

		public ActionResult GetShopCoupon(long shopId)
		{
			IEnumerable<CouponInfo> topCoupon = ServiceHelper.Create<ICouponService>().GetTopCoupon(shopId, 5, PlatformType.PC);
			return PartialView("_ShopCoupon", topCoupon);
		}

		public ActionResult Header()
		{
			List<ProductBrowsedHistoryModel> productBrowsedHistoryModels;
			bool currentUser = base.CurrentUser != null;
			base.ViewBag.isLogin = (currentUser ? "true" : "false");
			base.ViewBag.MemberIntegral = (currentUser ? ServiceHelper.Create<IMemberIntegralService>().GetMemberIntegral(base.CurrentUser.Id).AvailableIntegrals : 0);
			List<FavoriteInfo> favoriteInfos = (currentUser ? ServiceHelper.Create<IProductService>().GetUserAllConcern(base.CurrentUser.Id) : new List<FavoriteInfo>());
			ViewBag.Concern = favoriteInfos.Take(10).ToList();
			List<UserCouponInfo> userCouponInfos = (currentUser ? ServiceHelper.Create<ICouponService>().GetAllUserCoupon(base.CurrentUser.Id).ToList() : new List<UserCouponInfo>());
			userCouponInfos = (userCouponInfos == null ? new List<UserCouponInfo>() : userCouponInfos);
			ViewBag.Coupons = userCouponInfos;
			List<ShopBonusReceiveInfo> shopBonusReceiveInfos = (currentUser ? ServiceHelper.Create<IShopBonusService>().GetCanUseDetailByUserId(base.CurrentUser.Id) : new List<ShopBonusReceiveInfo>());
			shopBonusReceiveInfos = (shopBonusReceiveInfos == null ? new List<ShopBonusReceiveInfo>() : shopBonusReceiveInfos);
			ViewBag.ShopBonus = shopBonusReceiveInfos;
			productBrowsedHistoryModels = (currentUser ? BrowseHistrory.GetBrowsingProducts(10, (base.CurrentUser == null ? 0 : base.CurrentUser.Id)) : new List<ProductBrowsedHistoryModel>());
			ViewBag.BrowsingProducts = productBrowsedHistoryModels;
            InitHeaderData();
			return base.PartialView("~/Areas/Web/Views/Shared/Header.cshtml");
		}

		private void InitHeaderData()
		{
			string[] strArrays;
			ViewBag.SiteName = base.CurrentSiteSetting.SiteName;
			ViewBag.Logo = base.CurrentSiteSetting.Logo;
			ViewBag.Keyword = base.CurrentSiteSetting.Keyword;
			dynamic viewBag = base.ViewBag;
			strArrays = (!string.IsNullOrWhiteSpace(base.CurrentSiteSetting.Hotkeywords) ? base.CurrentSiteSetting.Hotkeywords.Split(new char[] { ',' }) : new string[0]);
			viewBag.HotKeyWords = strArrays;
			ViewBag.Navigators = ServiceHelper.Create<INavigationService>().GetPlatNavigations().ToArray();
			IEnumerable<HomeCategorySet> homeCategorySets = ServiceHelper.Create<IHomeCategoryService>().GetHomeCategorySets();
			ViewBag.Categories = homeCategorySets;
			ICategoryService categoryService = ServiceHelper.Create<ICategoryService>();
			dynamic array = base.ViewBag;
			IEnumerable<CategoryInfo> firstAndSecondLevelCategories = categoryService.GetFirstAndSecondLevelCategories();
			array.AllSecondCategoies = (
				from item in firstAndSecondLevelCategories
				where item.Depth == 2
				select item).ToArray();
			ServiceHelper.Create<IBrandService>();
			Dictionary<int, IEnumerable<BrandInfo>> nums = new Dictionary<int, IEnumerable<BrandInfo>>();
			ViewBag.PageFoot = base.CurrentSiteSetting.PageFoot;
			ViewBag.CategoryBrands = nums;
			ViewBag.Member = base.CurrentUser;
		}

		public ActionResult Logo()
		{
			return base.Content(base.CurrentSiteSetting.Logo);
		}

		public ActionResult ShopHeader()
		{
            InitHeaderData();
			return base.PartialView("~/Areas/Web/Views/Shared/ShopHeader.cshtml");
		}

		[ChildActionOnly]
		public ActionResult TopInfo()
		{
			ViewBag.SiteName = base.CurrentSiteSetting.SiteName;
			UserMemberInfo currentUser = base.CurrentUser;
			ViewBag.QR = base.CurrentSiteSetting.QRCode;
			return PartialView("~/Areas/Web/Views/Shared/_TopInfo.cshtml", currentUser);
		}
	}
}