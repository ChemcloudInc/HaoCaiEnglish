using Himall.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Web.Models
{
	public class ShopCartItemModel
	{
		public List<CartItemModel> CartItemModels
		{
			get;
			set;
		}

		public decimal Freight
		{
			get;
			set;
		}

		public long shopId
		{
			get;
			set;
		}

		public string ShopName
		{
			get;
			set;
		}

		public List<ShopBonusReceiveInfo> UserBonus
		{
			get;
			set;
		}

		public List<CouponRecordInfo> UserCoupons
		{
			get;
			set;
		}

		public ShopCartItemModel()
		{
            CartItemModels = new List<CartItemModel>();
            UserCoupons = new List<CouponRecordInfo>();
		}
	}
}