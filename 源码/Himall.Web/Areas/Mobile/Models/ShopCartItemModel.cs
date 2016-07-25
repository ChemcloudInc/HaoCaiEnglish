using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Mobile.Models
{
	public class ShopCartItemModel
	{
		public List<CartItemModel> CartItemModels
		{
			get;
			set;
		}

		public CouponModel Coupon
		{
			get;
			set;
		}

		public decimal Freight
		{
			get;
			set;
		}

		public bool isFreeFreight
		{
			get;
			set;
		}

		public decimal shopFreeFreight
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

		public long vshopId
		{
			get;
			set;
		}

		public ShopCartItemModel()
		{
            CartItemModels = new List<CartItemModel>();
		}
	}
}