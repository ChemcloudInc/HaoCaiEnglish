using System;
using System.Runtime.CompilerServices;

namespace Himall.Web.Areas.Mobile.Models
{
	public class CouponModel
	{
		public long CouponId
		{
			get;
			set;
		}

		public string CouponName
		{
			get;
			set;
		}

		public decimal CouponPrice
		{
			get;
			set;
		}

		public int Type
		{
			get;
			set;
		}

		public CouponModel()
		{
		}
	}
}