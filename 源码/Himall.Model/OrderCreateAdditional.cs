using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class OrderCreateAdditional
	{
		public ShippingAddressInfo Address
		{
			get;
			set;
		}

		public IEnumerable<BaseAdditionalCoupon> BaseCoupons
		{
			get;
			set;
		}

		public IEnumerable<CouponRecordInfo> Coupons
		{
			get;
			set;
		}

		public DateTime CreateDate
		{
			get;
			set;
		}

		public decimal IntegralTotal
		{
			get;
			set;
		}

		public OrderCreateAdditional()
		{
		}
	}
}