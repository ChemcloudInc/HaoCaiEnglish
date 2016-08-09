using Himall.Model;
using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Mobile.Models
{
	public class VshopCouponInfoModel
	{
		public int? AcceptId
		{
			get;
			set;
		}

		public CouponInfo CouponData
		{
			get;
			set;
		}

		public long CouponId
		{
			get;
			set;
		}

		public long CouponRecordId
		{
			get;
			set;
		}

		public CouponInfo.CouponReceiveStatus CouponStatus
		{
			get;
			set;
		}

		public string FollowUrl
		{
			get;
			set;
		}

		public bool IsFavoriteShop
		{
			get;
			set;
		}

		public bool IsShowSyncWeiXin
		{
			get;
			set;
		}

		public long ShopId
		{
			get;
			set;
		}

		public long VShopid
		{
			get;
			set;
		}

		public string WeiXinReceiveUrl
		{
			get;
			set;
		}

		public WXJSCardModel WXJSCardInfo
		{
			get;
			set;
		}

		public WXSyncJSInfoByCard WXJSInfo
		{
			get;
			set;
		}

		public VshopCouponInfoModel()
		{
		}
	}
}