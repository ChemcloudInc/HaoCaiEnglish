using Himall.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class MyAssetViewModel
	{
		public bool isLogin
		{
			get;
			set;
		}

		public List<ProductBrowsedHistoryModel> MyBrowsingProducts
		{
			get;
			set;
		}

		public List<FavoriteInfo> MyConcernsProducts
		{
			get;
			set;
		}

		public int MyCouponCount
		{
			get;
			set;
		}

		public List<UserCouponInfo> MyCoupons
		{
			get;
			set;
		}

		public int MyMemberIntegral
		{
			get;
			set;
		}

		public List<ShopBonusReceiveInfo> MyShopBonus
		{
			get;
			set;
		}

		public MyAssetViewModel()
		{
		}
	}
}