using Himall.IServices;
using Himall.Model;
using Himall.Web.Areas.Web;
using Himall.Web.Areas.Web.Models;
using Himall.Web.Framework;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Himall.Web.Areas.Web.Controllers
{
	public class SideController : BaseWebController
	{
		public SideController()
		{
		}

		public ActionResult MyAsset()
		{
			MyAssetViewModel myAssetViewModel = new MyAssetViewModel()
			{
				MyCouponCount = 0,
				isLogin = base.CurrentUser != null
			};
			base.ViewBag.isLogin = (myAssetViewModel.isLogin ? "true" : "false");
			myAssetViewModel.MyMemberIntegral = (myAssetViewModel.isLogin ? ServiceHelper.Create<IMemberIntegralService>().GetMemberIntegral(base.CurrentUser.Id).AvailableIntegrals : 0);
			List<FavoriteInfo> favoriteInfos = (myAssetViewModel.isLogin ? ServiceHelper.Create<IProductService>().GetUserAllConcern(base.CurrentUser.Id) : new List<FavoriteInfo>());
			myAssetViewModel.MyConcernsProducts = favoriteInfos.Take(10).ToList();
			List<UserCouponInfo> userCouponInfos = (myAssetViewModel.isLogin ? ServiceHelper.Create<ICouponService>().GetAllUserCoupon(base.CurrentUser.Id).ToList() : new List<UserCouponInfo>());
			userCouponInfos = (userCouponInfos == null ? new List<UserCouponInfo>() : userCouponInfos);
			myAssetViewModel.MyCoupons = userCouponInfos;
			MyAssetViewModel myCouponCount = myAssetViewModel;
			myCouponCount.MyCouponCount = myCouponCount.MyCouponCount + myAssetViewModel.MyCoupons.Count();
			List<ShopBonusReceiveInfo> shopBonusReceiveInfos = (myAssetViewModel.isLogin ? ServiceHelper.Create<IShopBonusService>().GetCanUseDetailByUserId(base.CurrentUser.Id) : new List<ShopBonusReceiveInfo>());
			shopBonusReceiveInfos = (shopBonusReceiveInfos == null ? new List<ShopBonusReceiveInfo>() : shopBonusReceiveInfos);
			myAssetViewModel.MyShopBonus = shopBonusReceiveInfos;
			MyAssetViewModel myCouponCount1 = myAssetViewModel;
			myCouponCount1.MyCouponCount = myCouponCount1.MyCouponCount + myAssetViewModel.MyShopBonus.Count();
			myAssetViewModel.MyBrowsingProducts = (myAssetViewModel.isLogin ? BrowseHistrory.GetBrowsingProducts(10, (base.CurrentUser == null ? 0 : base.CurrentUser.Id)) : new List<ProductBrowsedHistoryModel>());
			return View(myAssetViewModel);
		}
	}
}