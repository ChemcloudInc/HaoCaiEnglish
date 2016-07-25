using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class UserCenterModel
	{
		public decimal Expenditure
		{
			get;
			set;
		}

		public int FollowProductCount
		{
			get;
			set;
		}

		public List<FollowShopCart> FollowShopCarts
		{
			get;
			set;
		}

		public int FollowShopCartsCount
		{
			get;
			set;
		}

		public List<FollowShop> FollowShops
		{
			get;
			set;
		}

		public int FollowShopsCount
		{
			get;
			set;
		}

		public List<FollowProduct> FollwProducts
		{
			get;
			set;
		}

		public string GradeName
		{
			get;
			set;
		}

		public int Intergral
		{
			get;
			set;
		}

		public MemberAccountSafety memberAccountSafety
		{
			get;
			set;
		}

		public IQueryable<OrderInfo> Orders
		{
			get;
			set;
		}

		public int RefundCount
		{
			get;
			set;
		}

		public int UserCoupon
		{
			get;
			set;
		}

		public long WaitEvaluationOrders
		{
			get;
			set;
		}

		public long WaitPayOrders
		{
			get;
			set;
		}

		public long WaitReceivingOrders
		{
			get;
			set;
		}

		public UserCenterModel()
		{
            FollwProducts = new List<FollowProduct>();
            FollowShops = new List<FollowShop>();
            FollowShopCarts = new List<FollowShopCart>();
		}
	}
}